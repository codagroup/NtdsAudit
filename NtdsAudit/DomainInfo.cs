namespace CODA.NtdsAudit
{
    using System.Diagnostics;

    /// <summary>
    /// Provides information extracted from NTDS related to a domain.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class DomainInfo
    {
        /// <summary>
        /// Gets or sets the SID of the Administrators group.
        /// </summary>
        public MockSid AdministratorsSid { get; set; } = new(MockSidType.NullSid, null);

        /// <summary>
        /// Gets or sets the Distinguised Name of the domain.
        /// </summary>
        public string Dn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SID of the Domain Admin group.
        /// </summary>
        public MockSid DomainAdminsSid { get; set; } = new(MockSidType.NullSid, null);

        /// <summary>
        /// Gets or sets the SID of the Enterprise Admins group.
        /// </summary>
        public MockSid EnterpriseAdminsSid { get; set; } = new(MockSidType.NullSid, null);

        /// <summary>
        /// Gets or sets the SID of the Schema Admins group.
        /// </summary>
        public MockSid SchemaAdminsSid { get; set; } = new(MockSidType.NullSid, null);

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
        public MockSid Sid { get; set; } = new(MockSidType.NullSid, null);
    }
}
