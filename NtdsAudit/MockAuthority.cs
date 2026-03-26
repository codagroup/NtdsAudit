namespace CODA.NtdsAudit
{
    /// <summary>
    /// Structural representation of System.Security.Principal.Windows.IdentifierAuthority, but without all the Windows-specific logic attached.
    /// </summary>
    public enum MockAuthority : byte
    {
        NullAuthority = 0x000000,
        WorldAuthority = 0x000001,
        LocalAuthority = 0x000002,
        CreatorAuthority = 0x000003,
        NonUniqueAuthority = 0x000004,
        NTAuthority = 0x000005,
        SiteServerAuthority = 0x000006,
        InternetSiteAuthority = 0x000007,
        ExchangeAuthority = 0x000008,
        ResourceManagerAuthority = 0x000009,
        MandatoryLabelAuthority = 0x000016,
    }
}
