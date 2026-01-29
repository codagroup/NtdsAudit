namespace CODA.NtdsAudit
{
    using System.Diagnostics;
    using System.Security.Principal;

    /// <summary>
    /// Provides information extracted from NTDS in relation to a group.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class GroupInfo
    {
        /// <summary>
        /// Gets or sets the Distinguished Name.
        /// </summary>
        public string Dn { get; set; }

        /// <summary>
        /// Gets or sets the Distinguished Name Tag.
        /// </summary>
        public int Dnt { get; set; }

        /// <summary>
        /// Gets or sets the SID of the domain the group belongs to.
        /// </summary>
        public SecurityIdentifier DomainSid { get; set; }

        /// <summary>
        /// Gets or sets the list of DNTs of group members.
        /// </summary>
        public int[] MembersDnts { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the recursive list of DNTs of group members.
        /// </summary>
        public int[] RecursiveMembersDnts { get; set; }

        /// <summary>
        /// Gets or sets the SID.
        /// </summary>
        public SecurityIdentifier Sid { get; set; }
    }
}
