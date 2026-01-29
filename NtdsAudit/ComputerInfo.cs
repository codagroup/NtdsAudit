namespace CODA.NtdsAudit
{
    using System;
    using System.Diagnostics;
    using System.Security.Principal;

    /// <summary>
    /// Provides information extracted from NTDS in relation to a computer account.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class ComputerInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether the account is disabled.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the Distinguished Name.
        /// </summary>
        public string Dn { get; set; }

        /// <summary>
        /// Gets or sets the SID of the domain to which the account belongs.
        /// </summary>
        public SecurityIdentifier DomainSid { get; set; }

        /// <summary>
        /// Gets or sets the last logon date and time.
        /// </summary>
        public DateTime LastLogon { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}
