using System.Diagnostics.CodeAnalysis;
using System.Text;

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
        private int[]? _subAuthorities;
        private byte[]? _binaryForm;
        private MockSid? _accountDomainSid;
        #endregion
        #region Constructors
        public MockSid(string sddlForm)
        {
            ArgumentNullException.ThrowIfNull(sddlForm, nameof(sddlForm));
            byte[]? sidBinary = CreateSidFromString(sddlForm);
            CreateFromBinaryForm(sidBinary!, 0);
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

            // For some sidTypes, the domainSid parameter must be specified. no other validation is performed.
            if (IsDomainSid(sidType) && domainSid is null)
            {
                throw new ArgumentNullException(nameof(domainSid), $"When sidType is {sidType.ToString()} domainSid cannot be null");
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
        private bool IsDomainSid(MockSidType sidType)
        {
            switch (sidType)
            {
                case MockSidType.DomainAdministratorSid:
                case MockSidType.DomainGuestSid:
                case MockSidType.DomainKrbtgtSid:
                case MockSidType.DomainAdminsSid:
                case MockSidType.DomainUsersSid:
                case MockSidType.DomainGuestsSid:
                case MockSidType.DomainComputersSid:
                case MockSidType.DomainControllersSid:
                case MockSidType.DomainCertAdminsSid:
                case MockSidType.SchemaAdminsSid:
                case MockSidType.EnterpriseAdminsSid:
                case MockSidType.DomainPolicyAdminsSid:
                case MockSidType.DomainRasAndIasServersSid:
                case MockSidType.WinEnterpriseReadonlyControllersSid:
                case MockSidType.WinDomainReadonlyControllersSid:
                case MockSidType.WinEnterpriseReadonlyControllerSid:
                    return true;
                default:
                    return false;
            }
        }
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

            int totalLength = (4 * subAuthoritiesLength) + 8;
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

            CreateFromParts(authority, subAuthorities[..subAuthoritiesLength]);
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
            
            _binaryForm = new byte[(4 * _subAuthorities.Length) + 8];
            
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

            UpdateSddlForm();
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
            if (string.IsNullOrEmpty(sddlForm)) throw new ArgumentException("SID cannot be null");

            string[] parts = sddlForm.Split('-');
            if (parts[0] != "S" || parts.Length < 3) { throw new FormatException("Invalid SID format."); }

            byte revision = byte.Parse(parts[1]);
            UInt64 authority = UInt64.Parse(parts[2]);
            int subAuthorityCount = parts.Length - 3;

            // Total Size: 1 (Rev) + 1 (Count) + 6 (Auth) + (SubCount * 4)
            byte[] binarySid = new byte[8 + (subAuthorityCount * 4)];

            // 1. Set Revision
            binarySid[0] = revision;

            // 2. Set Sub-Authority Count
            binarySid[1] = (byte)subAuthorityCount;

            // 3. Set Identifier Authority (6 bytes, Big-Endian)
            for (int i = 0; i < 6; i++)
            {
                binarySid[2 + i] = (byte)((authority >> (8 * (5 - i))) & 0xFF);
            }

            // 4. Set Sub-Authorities (4 bytes each, Little-Endian)
            for (int i = 0; i < subAuthorityCount; i++)
            {
                uint subAuth = uint.Parse(parts[3 + i]);
                byte[] subAuthBytes = BitConverter.GetBytes(subAuth);

                // Ensure Little-Endian (Windows standard)
                if (!BitConverter.IsLittleEndian) Array.Reverse(subAuthBytes);

                Buffer.BlockCopy(subAuthBytes, 0, binarySid, 8 + (i * 4), 4);
            }

            return binarySid;
        }
        // Barely any error checking here.....
        internal byte[]? CreateWellKnownSid(MockSidType mockSidType, MockSid? domainSid)
        {
            //If the SID to be generated is on the pre-defined list and either the domain is null (i.e. it's a local SID) or the domain SID is valid:
            if (MockSecurity.WellKnownSids.Any(sid => sid.SidType == mockSidType) && (domainSid is null || domainSid.SubAuthorities.Length <= MaxSubAuthorities))
            {
                MockSecurity.WKSID wkSID = MockSecurity.WellKnownSids.First(sid => sid.SidType == mockSidType);
                if (domainSid is null)
                {
                    byte size = (byte)((wkSID.Sid.SubAuthorityCount * 4) + 8);
                    byte[] sidBytes = new byte[size];
                    sidBytes[0] = wkSID.Sid.Revision;
                    sidBytes[1] = wkSID.Sid.SubAuthorityCount;
                    sidBytes[7] = (byte)wkSID.Sid.Authority;

                    for (int i = 0; i < wkSID.Sid.SubAuthorityCount; i++)
                    {
                        byte[] subAuthBytes = BitConverter.GetBytes(wkSID.Sid.SubAuthority[i]);
                        if (!BitConverter.IsLittleEndian) Array.Reverse(subAuthBytes);

                        Array.Copy(subAuthBytes, 0, sidBytes, 8 + (i * 4), 4);
                    }
                    return sidBytes;
                }
                else
                {
                    byte size = (byte)((domainSid.SubAuthorities.Length * 4) + 8);
                    byte[] sidBytes = new byte[size];
                    sidBytes[0] = wkSID.Sid.Revision;
                    sidBytes[1] = (byte)domainSid.SubAuthorities.Length;
                    sidBytes[7] = (byte)domainSid.Authority;

                    for (int i = 0; i < domainSid.SubAuthorities.Length; i++)
                    {
                        byte[] subAuthBytes = BitConverter.GetBytes(domainSid.SubAuthorities[i]);
                        if (!BitConverter.IsLittleEndian) Array.Reverse(subAuthBytes);

                        Array.Copy(subAuthBytes, 0, sidBytes, 8 + (i * 4), 4);
                    }
                    return sidBytes;
                }
            }
            else return null;
        }
        internal void UpdateSddlForm()
        {
            if (_binaryForm is not null && _binaryForm.Length >= 8)
            {
                // Get Sub-Authority Count (Byte 1)
                byte subAuthorityCount = _binaryForm[1];

                // Validate that the array is long enough for the claimed sub-authorities
                if (_binaryForm.Length < 8 + (subAuthorityCount * 4)) { throw new ArgumentException("Binary array is too short for the specified sub-authority count."); }

                // Start building the string: S-Rev-Auth
                StringBuilder sb = new StringBuilder($"S-{Revision}-{(byte)_authority}");

                if (subAuthorityCount == 0)
                {
                    sb.Append($"-0");
                }
                else
                {
                    // Get Sub-Authorities (4 bytes each, Little-Endian)
                    for (int i = 0; i < subAuthorityCount; i++)
                    {
                        int offset = 8 + (i * 4);
                        uint subAuth = BitConverter.ToUInt32(_binaryForm, offset);

                        // Handle Big-Endian systems
                        if (!BitConverter.IsLittleEndian)
                        {
                            byte[] bytes = BitConverter.GetBytes(subAuth);
                            Array.Reverse(bytes);
                            subAuth = BitConverter.ToUInt32(bytes, 0);
                        }

                        sb.Append($"-{subAuth}");
                    }
                }
                SddlForm = sb.ToString();
            }
            else
            {
                throw new ArgumentException($"Invalid Binary SID");
            }
        }
        #endregion
        #region Properties
        public static byte Revision { get; private set; } = 1; //This is usually 1, but sometimes 0 or 2 because reasons.
        public static readonly int MinBinaryLength = 8;
        public string SddlForm
        {
            get; private set;
        } = String.Empty;
        public string Value { get { return SddlForm.ToUpperInvariant(); } }
        public MockSid? AccountDomainSid { get {
                if (_accountDomainSid is null)
                {
                    _accountDomainSid = GetAccountDomainSid();
                }
                return _accountDomainSid;
            } }
        public int BinaryLength { get { return _binaryForm!.Length; } }
        public MockAuthority Authority {  get { return _authority;} }
        public int[] SubAuthorities { get { return _subAuthorities!; } }
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
            for (int i = 0; i < _subAuthorities!.Length; i++)
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
                int[] values = _subAuthorities!;
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