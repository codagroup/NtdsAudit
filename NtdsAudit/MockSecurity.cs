namespace CODA.NtdsAudit
{
    public static class MockSecurity
    {
        const byte SID_REVISION = 0x01;
        public struct MAX_SID
        {
            public readonly byte Revision;
            public readonly byte SubAuthorityCount;
            public readonly MockAuthority Authority;
            public readonly ulong[] SubAuthority;
            public MAX_SID(byte revision, byte subAuthorityCount, MockAuthority authority, ulong[] subAuthority)
            {
                Revision = revision;
                SubAuthorityCount = subAuthorityCount;
                Authority = authority;
                SubAuthority = subAuthority;
            }
        }
        public struct WKSID
        {
            public readonly char[] wstr;
            public readonly MockSidType SidType;
            public readonly MAX_SID Sid;
            public WKSID(char[] wStr, MockSidType sidType, MAX_SID sid)
            {
                if (wStr.Length != 2) 
                {
                    throw new ArgumentOutOfRangeException("wStr", $"wStr must be a char[] of length 2"); 
                }
                wstr = wStr;
                SidType = sidType;
                Sid = sid;
            }
        }
        public readonly static WKSID[] WellKnownSids = {
            new(['0', '0'], MockSidType.NullSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NullAuthority, [(ulong)MockRid.NullRid])),
            new(['W', 'D'], MockSidType.WorldSid, new MAX_SID(SID_REVISION, 1, MockAuthority.WorldAuthority, [(ulong)MockRid.WorldRid])),
            new(['0', '0'], MockSidType.LocalSid, new MAX_SID(SID_REVISION, 1, MockAuthority.LocalAuthority, [(ulong)MockRid.LocalRid])),
            new(['C', 'O'], MockSidType.CreatorOwnerSid, new MAX_SID(SID_REVISION, 1, MockAuthority.CreatorAuthority, [(ulong)MockRid.CreatorOwnerRid])),
            new(['C', 'G'], MockSidType.CreatorGroupSid, new MAX_SID(SID_REVISION, 1, MockAuthority.CreatorAuthority, [(ulong)MockRid.CreatorGroupRid])),
            new(['0', '0'], MockSidType.CreatorOwnerServerSid, new MAX_SID(SID_REVISION, 1, MockAuthority.CreatorAuthority, [(ulong)MockRid.CreatorOwnerServerRid])),
            new(['0', '0'], MockSidType.CreatorGroupServerSid, new MAX_SID(SID_REVISION, 1, MockAuthority.CreatorAuthority, [(ulong)MockRid.CreatorGroupServerRid])),
            new(['0', '0'], MockSidType.NTAuthoritySid, new MAX_SID(SID_REVISION, 0, MockAuthority.NTAuthority, [(ulong)MockRid.NullRid])),
            new(['0', '0'], MockSidType.DialupSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.DialupRid])),
            new(['N', 'U'], MockSidType.NetworkSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.NetworkRid])),
            new(['0', '0'], MockSidType.BatchSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.BatchRid])),
            new(['I', 'U'], MockSidType.InteractiveSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.InteractiveRid])),
            new(['S', 'U'], MockSidType.ServiceSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.ServiceRid])),
            new(['A', 'N'], MockSidType.AnonymousSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.AnonymousLogonRid])),
            new(['0', '0'], MockSidType.ProxySid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.ProxyRid])),
            new(['E', 'D'], MockSidType.EnterpriseControllersSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.EnterpriseControllersRid])),
            new(['P', 'S'], MockSidType.SelfSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.PrincipalSelfRid])),
            new(['A', 'U'], MockSidType.AuthenticatedUserSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.AuthenticatedUserRid])),
            new(['R', 'C'], MockSidType.RestrictedCodeSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.RestrictedCodeRid])),
            new(['0', '0'], MockSidType.TerminalServerSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.TerminalServerRid])),
            new(['0', '0'], MockSidType.RemoteLogonIdSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.RemoteLogonRid])),
            new(['S', 'Y'], MockSidType.LocalSystemSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.LocalSystemRid])),
            new(['L', 'S'], MockSidType.LocalServiceSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.LocalServiceRid])),
            new(['N', 'S'], MockSidType.NetworkServiceSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.NetworkServiceRid])),
            new(['0', '0'], MockSidType.BuiltinDomainSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid])),
            new(['B', 'A'], MockSidType.BuiltinAdministratorsSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidAdmins])),
            new(['B', 'U'], MockSidType.BuiltinUsersSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidUsers])),
            new(['B', 'G'], MockSidType.BuiltinGuestsSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidGuests])),
            new(['P', 'U'], MockSidType.BuiltinPowerUsersSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidPowerUsers])),
            new(['A', 'O'], MockSidType.BuiltinAccountOperatorsSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidAccountOperators])),
            new(['S', 'O'], MockSidType.BuiltinSystemOperatorsSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidSystemOperators])),
            new(['P', 'O'], MockSidType.BuiltinPrintOperatorsSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidPrintOperators])),
            new(['B', 'O'], MockSidType.BuiltinBackupOperatorsSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidBackupOperators])),
            new(['R', 'E'], MockSidType.BuiltinReplicatorSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidReplicator])),
            new(['R', 'U'], MockSidType.BuiltinPreWindows2000CompatibleAccessSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidPreW2K])),
            new(['R', 'D'], MockSidType.BuiltinRemoteDesktopUsersSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidRDSUsers])),
            new(['N', 'O'], MockSidType.BuiltinNetworkConfigurationOperatorsSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidNetworkConfigurationOperators])),
            new(['0', '0'], MockSidType.NtlmAuthenticationSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.SecurityPackageBaseRid, (ulong)MockRid.SecurityPackageNTLMRid])),
            new(['0', '0'], MockSidType.DigestAuthenticationSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.SecurityPackageBaseRid, (ulong)MockRid.SecurityPackageDigestRid])),
            new(['0', '0'], MockSidType.SChannelAuthenticationSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.SecurityPackageBaseRid, (ulong)MockRid.SecurityPackageSChannelRid])),
            new(['0', '0'], MockSidType.ThisOrganizationSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.ThisOrganizationRid])),
            new(['0', '0'], MockSidType.OtherOrganizationSid, new MAX_SID(SID_REVISION, 1, MockAuthority.NTAuthority, [(ulong)MockRid.OtherOrganizationRid])),
            new(['0', '0'], MockSidType.BuiltinIncomingForestTrustBuildersSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidIncomingForestTrustBuilders])),
            new(['0', '0'], MockSidType.BuiltinPerformanceMonitoringUsersSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidMonitoringUsers])),
            new(['0', '0'], MockSidType.BuiltinPerformanceLoggingUsersSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidLoggingUsers])),
            new(['0', '0'], MockSidType.BuiltinAuthorizationAccessSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidAuthorizationAccess])),
            new(['0', '0'], MockSidType.WinBuiltinTerminalServerLicenseServersSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidRDSLicenceServers])),
            new(['0', '0'], MockSidType.WinBuiltinDCOMUsersSid, new MAX_SID(SID_REVISION, 2, MockAuthority.NTAuthority, [(ulong)MockRid.BuiltinDomainRid, (ulong)MockRid.DomainAliasRidDCOMUsers])),
            new(['L', 'W'], MockSidType.WinLowLabelSid, new MAX_SID(SID_REVISION, 1, MockAuthority.MandatoryLabelAuthority, [(ulong)MockRid.SecurityMandatoryLowRid])),
            new(['M', 'E'], MockSidType.WinMediumLabelSid, new MAX_SID(SID_REVISION, 1, MockAuthority.MandatoryLabelAuthority, [(ulong)MockRid.SecurityMandatoryMediumRid])),
            new(['H', 'I'], MockSidType.WinHighLabelSid, new MAX_SID(SID_REVISION, 1, MockAuthority.MandatoryLabelAuthority, [(ulong)MockRid.SecurityMandatoryHighRid])),
            new(['S', 'I'], MockSidType.WinSystemLabelSid, new MAX_SID(SID_REVISION, 1, MockAuthority.MandatoryLabelAuthority, [(ulong)MockRid.SecurityMandatorySystemRid]))
        };
        public static int Length { get { return WellKnownSids.Length; } }
    }
}
