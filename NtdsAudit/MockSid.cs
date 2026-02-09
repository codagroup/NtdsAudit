using Microsoft.Isam.Esent.Interop;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace CODA.NtdsAudit
{
    /// <summary>
    /// Structural replica of the System.Security.Principal.Windows.SecurityIdentifier class, but without all the Windows-specific logic attached.
    /// </summary>
    public class MockSid
    {
        // SID Structure: S-1-5-21-1999943590-2734198879-4288172524-500 from testlab administrator
        // 1 => Revision 1
        // 5 => 0-based number of fields in domain SID, 1 byte, then 6 bytes big-endian
        // 21 => Security Flag (non-unique) - 4 bytes little-endian
        // 1999943590 => Domain-SID part 1 - 4 bytes little-endian
        // 2734198879 => Domain-SID part 2 - 4 bytes little-endian
        // 4288172524 => Domain-SID part 3 - 4 bytes little-endian
        // 500 => User ID => unique within domain - 4 bytes little-endian
        // Bytes: {1,5,0,0,0,0,0,5,21,0,0,0,166,183,52,119,95,144,248,162,236,81,152,255,244,1,0,0}
        // - Revision = 1
        // - SIDfields-1 = 5
        // - SIDfields-1 = 5 big-endian => 0x000000000005
        // - Security flag = 21 little-endian => 0x21000000
        // - Part 1 = 1999943590 little-endian => 0xA6B73477 => 166,183,52,119
        // - Part 2 = 2734198879 little-endian => 0x5F90F8A2 => 95,144,248,162
        // - Part 3 = 4288172524 little-endian => 0xEC5198FF => 236,81,152,255
        // - User ID = 500 little-endian => 0xF4010000 => 244,1,0,0

        #region Fields
        internal const int MaxSubAuthorities = 15;
        internal const long MaxIdentifierAuthority = 0xFFFFFFFFFFFF;
        private MockAuthority _authority;
        private int[] _subAuthorities;
        private byte[] _binaryForm;
        private MockSid? _accountDomainSid;
        #endregion
        #region Constructors
        public MockSid(string sddlForm)
        {
            ArgumentNullException.ThrowIfNull(sddlForm, nameof(sddlForm));
            byte[]? sidBinary = CreateSidFromString(sddlForm);
            CreateFromBinaryForm(_binaryForm!, 0);
        }
        public MockSid(byte[] binaryForm, int offset)
        {
            ArgumentNullException.ThrowIfNull(binaryForm);
            CreateFromBinaryForm(binaryForm, offset);
        }
        public MockSid(MockSidType sidType, MockSid? domainSid)
        {
            // sidType must not be equal to LogonIdsSid
            if (sidType == MockSidType.LogonIdsSid)
            {
                throw new ArgumentException($"Cannot create a Logon SID.", nameof(sidType));
            }

            // sidType should not exceed the max defined value
            if ((sidType < MockSidType.NullSid) || (sidType > MockSidType.WinCapabilityRemovableStorageSid))
            {
                throw new ArgumentException($"An invalid sid type was provided: {sidType.ToString()}", nameof(sidType));
            }
            
            // For sidType between 38 to 50, the domainSid parameter must be specified. no other validation is performed.
            if ((sidType >= MockSidType.AccountAdministratorSid) && (sidType <= MockSidType.AccountRasAndIasServersSid))
            {
                if (domainSid == null)
                {
                    throw new ArgumentNullException(nameof(domainSid), $"When sidType is {sidType.ToString()} domainSid cannot be null");
                }
            }
            byte[]? binarySid = CreateWellKnownSid(sidType, domainSid);
            CreateFromBinaryForm(binarySid!, 0);
        }
        public MockSid(MockAuthority authority, ReadOnlySpan<int> subAuthorities)
        {
            CreateFromParts(authority, subAuthorities);
        }
        #endregion
        #region Functions
        private void CreateFromBinaryForm(byte[] binaryForm, int offset)
        {
            ArgumentNullException.ThrowIfNull(binaryForm);
            ArgumentOutOfRangeException.ThrowIfNegative(offset);

            if (binaryForm.Length - offset < MinBinaryLength)
            {
                throw new ArgumentOutOfRangeException(nameof(binaryForm), $"Binary form is too small");
            }
            if (binaryForm[offset] != Revision)
            {
                // Revision is incorrect
                throw new ArgumentException($"Binary form indicates incorrect revision: {binaryForm[offset].ToString()}", nameof(binaryForm));
            }

            // Insist on the correct number of subauthorities
            int subAuthoritiesLength = binaryForm[offset + 1];
            if (subAuthoritiesLength > MaxSubAuthorities)
            {
                throw new ArgumentException($"Incorrect number of subauthorities: {binaryForm[offset+1].ToString()}", nameof(binaryForm));
            }
            // Make sure the buffer is big enough

            int totalLength = 1 + 1 + 6 + 4 * subAuthoritiesLength;
            if (binaryForm.Length - offset < totalLength)
            {
                throw new ArgumentException($"Buffer too small: {binaryForm.Length - offset}", nameof(binaryForm));
            }

            Span<int> subAuthorities = stackalloc int[MaxSubAuthorities];
            MockAuthority authority = (MockAuthority)(
                (((long)binaryForm[offset + 2]) << 40) +
                (((long)binaryForm[offset + 3]) << 32) +
                (((long)binaryForm[offset + 4]) << 24) +
                (((long)binaryForm[offset + 5]) << 16) +
                (((long)binaryForm[offset + 6]) << 8) +
                (((long)binaryForm[offset + 7]))
            );

            // Subauthorities are represented in big-endian format
            for (int i = 0; i < subAuthoritiesLength; i++)
            {
                subAuthorities[i] =
                    (int)(
                    (((uint)binaryForm[offset + 8 + 4 * i + 0]) << 0) +
                    (((uint)binaryForm[offset + 8 + 4 * i + 1]) << 8) +
                    (((uint)binaryForm[offset + 8 + 4 * i + 2]) << 16) +
                    (((uint)binaryForm[offset + 8 + 4 * i + 3]) << 24)
                );
            }

            CreateFromParts(
                authority,
                subAuthorities[..subAuthoritiesLength]
            );

            return;
        }
        private void CreateFromParts(MockAuthority authority, ReadOnlySpan<int> subAuthorities)
        {
            //
            // Check the number of subauthorities passed in
            //
            if (subAuthorities.Length > MaxSubAuthorities)
            {
                throw new ArgumentOutOfRangeException(
                    "subAuthorities.Length",
                    subAuthorities.Length,
                    $"Number of subautorities cannot exceed {MaxSubAuthorities.ToString()}"
                );
            }
            
            //
            // Identifier authority is at most 6 bytes long
            //

            if (authority < 0 || (long)authority > MaxIdentifierAuthority)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(authority),
                    authority,
                    $"Authority length too long"
                );
            }
            
            //
            // Create a local copy of the data passed in
            //

            _authority = authority;
            _subAuthorities = subAuthorities.ToArray();
            
            //
            // Compute and store the binary form
            //
            // typedef struct _SID {
            //     UCHAR Revision;
            //     UCHAR SubAuthorityCount;
            //     SID_IDENTIFIER_AUTHORITY IdentifierAuthority;
            //     ULONG SubAuthority[ANYSIZE_ARRAY]
            // } SID, *PISID;
            //
            
            _binaryForm = new byte[1 + 1 + 6 + 4 * _subAuthorities.Length];
            
            //
            // First two bytes contain revision and subauthority count
            //

            _binaryForm[0] = Revision;
            _binaryForm[1] = (byte)_subAuthorities.Length;
            
            //
            // Identifier authority takes up 6 bytes
            //

            for (int i = 0; i < 6; i++)
            {
                _binaryForm[2 + i] = (byte)((((ulong)_authority) >> ((5 - i) * 8)) & 0xFF);
            }

            //
            // Subauthorities go last, preserving big-endian representation
            //

            for (int i = 0; i < _subAuthorities.Length; i++)
            {
                for (byte shift = 0; shift < 4; shift += 1)
                {
                    _binaryForm[8 + 4 * i + shift] = unchecked((byte)(((ulong)_subAuthorities[i]) >> (shift * 8)));
                }
            }
        }
        internal MockSid? GetAccountDomainSid()
        {
            MockSid? result = null;
            if (SddlForm.Length >= 41)
            {
                result = new MockSid(SddlForm.Substring(0, 41));
            }
            return result;
        }
        //TODO: This can be more efficient
        internal byte[]? CreateSidFromString(string sddlForm)
        {
            byte[]? binaryForm = null;
            if (sddlForm.ToUpperInvariant().StartsWith("S") && sddlForm.Length >= MinBinaryLength)
            {
                string[] sddlParts = sddlForm.Split('-');
                List<byte> byteParts = new List<byte>();
                if (sddlParts.Length >= MinBinaryLength/2)
                {
                    for (int i = 1; i < sddlParts.Length; i++) //Ignore the "S" string
                    {
                        uint u = 0;
                        bool result = uint.TryParse(sddlParts[i], out u);
                        if (!result)
                        {
                            throw new ArgumentException($"Invalid SID: {sddlForm}");
                        }
                        else
                        {
                            switch(i)
                            {
                                case 1: //Revision part (1 byte, generally equals "1", but could in theory be anything)
                                    if (u <= byte.MaxValue)
                                    {
                                        byteParts.Add((byte)u);
                                    }
                                    else
                                    {
                                        throw new ArgumentException($"Invalid SID: {sddlForm}");
                                    }
                                    break;
                                case 2: //Number of fields (1 byte then 6 bytes - varies, but typically equals "5")
                                    if (u <= byte.MaxValue)
                                    {
                                        for (int j = 0; j < 7; j++)
                                        {
                                            if (j == 0 || j == 6)
                                            {
                                                byteParts.Add((byte)u);
                                            }
                                            else
                                            {
                                                byteParts.Add(0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentException($"Invalid SID: {sddlForm}");
                                    }
                                break;
                                default: //All other sections are little-endian.
                                    byte[] b = BitConverter.GetBytes(u);
                                    if (!BitConverter.IsLittleEndian)
                                    {
                                        Array.Reverse(b);
                                    }
                                    byteParts.AddRange(b);
                                break;
                            }
                        }
                    }
                    binaryForm = byteParts.ToArray();
                }
                else
                {
                    throw new ArgumentException($"SID not long enough: {sddlForm} is shorter than {MinBinaryLength / 2}");
                }
            }
            return binaryForm;
        }
        // Barely any error checking here.....
        internal byte[]? CreateWellKnownSid(MockSidType mockSidType, MockSid? domainSid)
        {
            //TODO: Figure out this bit
            /*if (MockSecurity.WellKnownSids.Any(sid => sid.SidType == mockSidType) && 
                (domainSid is null || domainSid._subAuthorities.Count() <= MaxSubAuthorities)
            )
            {
                
                
            }
            else return null;
            */

            throw new NotImplementedException("CreateWellKnownSid");
        }
        #endregion
        #region Properties
        public static byte Revision { get; private set; } = 1; //This is usually 1, but sometimes 0 or 2 because reasons.
        public static readonly int MinBinaryLength = 8;
        public string SddlForm { get; private set; }
        public string Value { get { return SddlForm.ToUpperInvariant(); } }
        public MockSid? AccountDomainSid { get {
                if (_accountDomainSid is null)
                {
                    _accountDomainSid = GetAccountDomainSid();
                }
                return _accountDomainSid;
            } }
        public int BinaryLength { get { return _binaryForm.Length; } }
        #endregion
        #region Overrides
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return this == obj as MockSid;
        }
        public bool Equals(MockSid sid)
        {
            return this == sid;
        }
        public override int GetHashCode()
        {
            int hash = ((long)_authority).GetHashCode();
            for (int i = 0; i < _subAuthorities.Length; i++)
            {
                hash ^= _subAuthorities[i];
            }
            return hash;
        }
        public override string ToString()
        {
            if (SddlForm is null)
            {
                // Typecasting of _authority to a ulong below is important, since
                // otherwise you would see this: "S-1-NTAuthority-32-544"

                // length of buffer calculation
                // prefix = "S-1-".Length: 4;
                // authority: ulong.MaxValue.ToString("D") : 20;
                // subauth = MaxSubAuthorities * ( uint.MaxValue.ToString("D").Length + '-'.Length ): 15 * (10+1): 165;
                // max possible length = 4 + 20 + 165: 189
                Span<char> result = stackalloc char[189];
                result[0] = 'S';
                result[1] = '-';
                result[2] = '1';
                result[3] = '-';
                int length = 4;
                ((ulong)_authority).TryFormat(result.Slice(length), out int written);
                length += written;
                int[] values = _subAuthorities;
                for (int index = 0; index < values.Length; index++)
                {
                    result[length] = '-';
                    length += 1;
                    ((uint)values[index]).TryFormat(result.Slice(length), out written);
                    length += written;
                }
                SddlForm = result.Slice(0, length).ToString();
            }
            return SddlForm;
        }        
        #endregion
    }
}