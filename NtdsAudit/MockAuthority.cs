namespace CODA.NtdsAudit
{
    /// <summary>
    /// Structural representation of System.Security.Principal.Windows.IdentifierAuthority, but without all the Windows-specific logic attached.
    /// </summary>
    public enum MockAuthority:long
    {
        NullAuthority = 0,
        WorldAuthority = 1,
        LocalAuthority = 2,
        CreatorAuthority = 3,
        NonUniqueAuthority = 4,
        NTAuthority = 5,
        SiteServerAuthority = 6,
        InternetSiteAuthority = 7,
        ExchangeAuthority = 8,
        ResourceManagerAuthority = 9,
        MandatoryLabelAuthority = 16,
    }
}
