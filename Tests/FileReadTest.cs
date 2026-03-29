namespace Tests;

[TestFixture]
public class NTDSAudit_FileRead_Test
{
    #region Fields
    private readonly int domainCount = 1;
    private readonly int userCount = 103;
    private readonly int filteredUserCount = 10;
    private readonly int activeUserCount = 87;
    private readonly int filteredActiveUserCount = 10;
    private readonly int disabledUserCount = 16;
    private readonly int filteredDisabledUserCount = 0;
    private readonly int expiredUserCount = 0;
    private readonly int filteredExpiredUserCount = 0;
    private readonly int inactiveuser1YearCount = 86;
    private readonly int inactiveuser90DaysCount = 86;
    private readonly int filteredInactiveUserCount = 10;
    private readonly int userNoPasswordCount = 0;
    private readonly int filteredUserNoPasswordCount = 0;
    private readonly int userNonExpiryCount = 1;
    private readonly int filteredUserNonExpiryCount = 0;
    private readonly int userOldPassword1Year = 0;
    private readonly int userOldPassword90Days = 0;
    private readonly int filteredUserOldPassword = 0;
    private readonly int administratorCount = 1;
    private readonly int domainAdminCount = 1;
    private readonly int enterpriseAdminCount = 1;
    private readonly int schemaAdminCount = 1;
    private readonly int filteredAdministratorCount = 0;
    private readonly int computerCount = 101;
    private readonly int filteredComputerCount = 0;
    private readonly int disabledComputerCount = 0;
    private readonly int filteredDisabledComputerCount = 0;
    private readonly int inactiveComputer1YearCount = 100;
    private readonly int inactiveComputer90DayCount = 100;
    private readonly int filteredInactiveComputerCount = 0;
    #endregion
    #region Tests
    [Test]
    public void LoadValidNTDS_NoDump()
    {
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, false, false, TestHelpers.SystemHivePath, string.Empty, string.Empty);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(domainAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(enterpriseAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(schemaAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(computerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(disabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump()
    {
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, false, TestHelpers.SystemHivePath, string.Empty, string.Empty);
            
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(domainAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(enterpriseAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(schemaAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(computerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(disabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }

            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            Assert.That(TestHelpers.LMHash, Is.EqualTo(ntds.Users.First(u => u.Name.Equals("administrator", StringComparison.OrdinalIgnoreCase))?.LmHash));
            Assert.That(TestHelpers.NTHash, Is.EqualTo(ntds.Users.First(u => u.Name.Equals("administrator", StringComparison.OrdinalIgnoreCase))?.NtHash));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump_WithHistory()
    {
        //TODO: Need to create users with history hashes.
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, true, TestHelpers.SystemHivePath, string.Empty, string.Empty);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(domainAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(enterpriseAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(schemaAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(computerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(disabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }

            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.Equals("administrator", StringComparison.OrdinalIgnoreCase));

            if (userInfo is not null)
            {
                Assert.That(TestHelpers.LMHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(TestHelpers.NTHash, Is.EqualTo(userInfo?.NtHash));
            }
            else
            {
                Assert.Fail("administrator user account not found");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump_WithCrack()
    {
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, false, TestHelpers.SystemHivePath, TestHelpers.WordlistPath, string.Empty);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(domainAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(enterpriseAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(schemaAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(computerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(disabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }

            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.Equals("administrator", StringComparison.OrdinalIgnoreCase));

            if (userInfo is not null)
            {
                Assert.That(TestHelpers.LMHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(TestHelpers.NTHash, Is.EqualTo(userInfo?.NtHash));

                Assert.That(TestHelpers.DefaultPassword, Is.EqualTo(userInfo?.Password));
            }
            else
            {
                Assert.Fail("administrator user account not found");
            }            
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump_WithHistory_WithCrack()
    {
        //TODO: Need to create users with history hashes.
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, true, TestHelpers.SystemHivePath, TestHelpers.WordlistPath, string.Empty);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(domainAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(enterpriseAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(schemaAdminCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(computerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(disabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }

            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.Equals("administrator", StringComparison.OrdinalIgnoreCase));

            if (userInfo is not null)
            {
                Assert.That(TestHelpers.LMHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(TestHelpers.NTHash, Is.EqualTo(userInfo?.NtHash));

                Assert.That(TestHelpers.DefaultPassword, Is.EqualTo(userInfo?.Password));
            }
            else
            {
                Assert.Fail("administrator user account not found");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
    [Test]
    public void LoadValidNTDS_NoDump_OUFilter()
    {
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, false, false, TestHelpers.SystemHivePath, string.Empty, TestHelpers.OuFilterPath);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(filteredDisabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(filteredExpiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(filteredActiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(filteredUserNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(filteredUserNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(filteredComputerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(filteredDisabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump_OUFilter()
    {
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, false, TestHelpers.SystemHivePath, string.Empty, TestHelpers.OuFilterPath);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(filteredDisabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(filteredExpiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(filteredActiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(filteredUserNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(filteredUserNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(filteredComputerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(filteredDisabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }

            Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts
            UserInfo userInfo = ntds.Users.First();

            if (userInfo is not null)
            {
                Assert.That(TestHelpers.LMHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(TestHelpers.NTHash, Is.EqualTo(userInfo?.NtHash));
            }
            else
            {
                Assert.Fail("User accounts not found");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump_WithHistory_OUFilter()
    {
        //TODO: Need to create users with history hashes.
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, true, TestHelpers.SystemHivePath, string.Empty, TestHelpers.OuFilterPath);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(filteredDisabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(filteredExpiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(filteredActiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(filteredUserNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(filteredUserNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(filteredComputerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(filteredDisabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }

            Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First();

            if (userInfo is not null)
            {
                Assert.That(TestHelpers.LMHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(TestHelpers.NTHash, Is.EqualTo(userInfo?.NtHash));
            }
            else
            {
                Assert.Fail("User accounts not found");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump_WithCrack_OUFilter()
    {
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, false, TestHelpers.SystemHivePath, TestHelpers.WordlistPath, TestHelpers.OuFilterPath);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(filteredDisabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(filteredExpiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(filteredActiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(filteredUserNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(filteredUserNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(filteredComputerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(filteredDisabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }

            Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First();

            if (userInfo is not null)
            {
                Assert.That(TestHelpers.LMHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(TestHelpers.NTHash, Is.EqualTo(userInfo?.NtHash));

                Assert.That(TestHelpers.DefaultPassword, Is.EqualTo(userInfo?.Password));
            }
            else
            {
                Assert.Fail("User accounts not found");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump_WithHistory_WithCrack_OUFilter()
    {
        //TODO: Need to create users with history hashes.
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, true, TestHelpers.SystemHivePath, TestHelpers.WordlistPath, TestHelpers.OuFilterPath);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(filteredDisabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(filteredExpiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < TestHelpers.BaseDateTime)));
                Assert.That(filteredActiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveUserCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(filteredUserNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(filteredUserNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(filteredUserOldPassword, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].AdministratorsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].DomainAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].EnterpriseAdminsSid))));
                Assert.That(filteredAdministratorCount, Is.EqualTo(ntds.Users.Count(x => x.RecursiveGroupSids.Contains(ntds.Domains[0].SchemaAdminsSid))));
                Assert.That(filteredComputerCount, Is.EqualTo(ntds.Computers.Count()));
                Assert.That(filteredDisabledComputerCount, Is.EqualTo(ntds.Computers.Count(x => x.Disabled)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 365)));
                Assert.That(filteredInactiveComputerCount, Is.EqualTo(ntds.Computers.Count(x => !x.Disabled && GetAge(x.LastLogon) > 90)));
            }

            Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(filteredUserCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First();

            if (userInfo is not null)
            {
                Assert.That(TestHelpers.LMHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(TestHelpers.NTHash, Is.EqualTo(userInfo?.NtHash));

                Assert.That(TestHelpers.DefaultPassword, Is.EqualTo(userInfo?.Password));
            }
            else
            {
                Assert.Fail("User accounts not found");
            }
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
    #endregion
    #region Helpers
    private static UserInfo[] GetActiveUsers(NtdsAuditor ntds)
    {
        return [.. ntds.Users.Where(x => !x.Disabled && (!x.Expires.HasValue || x.Expires!.Value > TestHelpers.BaseDateTime))];
    }
    private static double GetAge(DateTime referenceDate)
    {
        return (TestHelpers.BaseDateTime - referenceDate).TotalDays;
    }
    #endregion
}