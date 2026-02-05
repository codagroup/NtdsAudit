namespace CODA.NtdsAudit
{
    using System.Diagnostics;
    using System.Security.Principal;

    /// <summary>
    /// Provides information extracted from NTDS related to a domain.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class DomainInfo
    {
        /// <summary>
        /// Gets or sets the SID of the Administrators group.
        /// </summary>
        public SecurityIdentifier AdministratorsSid { get; set; } = new(WellKnownSidType.NullSid, null);

        /// <summary>
        /// Gets or sets the Distinguised Name of the domain.
        /// </summary>
        public string Dn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SID of the Domain Admin group.
        /// </summary>
        public SecurityIdentifier DomainAdminsSid { get; set; } = new(WellKnownSidType.NullSid, null);

        /// <summary>
        /// Gets or sets the SID of the Enterprise Admins group.
        /// </summary>
        public SecurityIdentifier EnterpriseAdminsSid { get; set; } = new(WellKnownSidType.NullSid, null);

        /// <summary>
        /// Gets or sets the FQDN of the domain.
        /// </summary>
        public string Fqdn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the domain.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SID of the domain.
        /// </summary>
        public SecurityIdentifier Sid { get; set; } = new(WellKnownSidType.NullSid, null);
    }
}
