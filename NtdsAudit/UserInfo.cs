namespace CODA.NtdsAudit
{
    using System;
    using System.Diagnostics;
    using System.Security.Principal;

    /// <summary>
    /// Provides information extracted from NTDS in relation to a user account.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class UserInfo
    {
        /// <summary>
        /// Gets or sets the clear text password (passwords stored using reversible encryption).
        /// </summary>
        public string ClearTextPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the account is disabled.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the Distinguished Name.
        /// </summary>
        public string Dn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Distinguished Name Tag.
        /// </summary>
        public int Dnt { get; set; }

        /// <summary>
        /// Gets or sets the SID of the doamin the account belongs to.
        /// </summary>
        public SecurityIdentifier DomainSid { get; set; } = new (WellKnownSidType.NullSid, null);

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// Gets or sets the last logon date and time.
        /// </summary>
        public DateTime LastLogon { get; set; }

        /// <summary>
        /// Gets or sets the LM hash.
        /// </summary>
        public string LmHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the LM history hashes.
        /// </summary>
        public string[] LmHistory { get; set; } = [];

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the NT hash.
        /// </summary>
        public string NtHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the NT history hashes.
        /// </summary>
        public string[] NtHistory { get; set; } = [];

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date time the password was last changed.
        /// </summary>
        public DateTime PasswordLastChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the password is set to never expire.
        /// </summary>
        public bool PasswordNeverExpires { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a password is not required.
        /// </summary>
        public bool PasswordNotRequired { get; set; }

        /// <summary>
        /// Gets or sets the SIDs of groups of which the account is a member.
        /// </summary>
        public SecurityIdentifier[] RecursiveGroupSids { get; set; } = [];

        /// <summary>
        /// Gets or sets the Relative ID.
        /// </summary>
        public uint Rid { get; set; }

        /// <summary>
        /// Gets or sets the sam account name.
        /// </summary>
        public string SamAccountName { get; set; } = string.Empty;
    }
}
