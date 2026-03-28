namespace Tests;

[TestFixture]
public class NTDSAudit_FileRead_Test
{
    #region Fields
    private readonly int domainCount = 1;
    private readonly int userCount = 103;
    private readonly int activeUserCount = 87;
    private readonly int disabledUserCount = 16;
    private readonly int expiredUserCount = 0;
    private readonly int inactiveuser1YearCount = 86;
    private readonly int inactiveuser90DaysCount = 86;
    private readonly int userNoPasswordCount = 0;
    private readonly int userNonExpiryCount = 1;
    private readonly int userOldPassword1Year = 0;
    private readonly int userOldPassword90Days = 0;
    private readonly int administratorCount = 1;
    private readonly int domainAdminCount = 1;
    private readonly int enterpriseAdminCount = 1;
    private readonly int computerCount = 101;
    private readonly int disableComputerCount = 0;
    private readonly int inactiveComputer1YearCount = 100;
    private readonly int inactiveComputer90DayCount = 100;
    private readonly DateTime baseDateTime = new(2026, 01, 23, 14, 0, 32, DateTimeKind.Utc);
    private readonly string lmHash = "AAD3B435B51404EEAAD3B435B51404EE"; //Empty LM Hash.
    private readonly string ntHash = "DAE57D78FEC919471799CE0FAE8236B9"; //Pa55w0rd!
    private readonly string defaultPassword = "Pa55w0rd!";
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
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
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
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
            }

            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            Assert.That(lmHash, Is.EqualTo(ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator")?.LmHash));
            Assert.That(ntHash, Is.EqualTo(ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator")?.NtHash));
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
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
            }

            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator");

            if (userInfo is not null)
            {
                Assert.That(lmHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(ntHash, Is.EqualTo(userInfo?.NtHash));
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
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
            }


            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator");

            if (userInfo is not null)
            {
                Assert.That(lmHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(ntHash, Is.EqualTo(userInfo?.NtHash));

                Assert.That(defaultPassword, Is.EqualTo(userInfo?.Password));
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
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
            }


            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator");

            if (userInfo is not null)
            {
                Assert.That(lmHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(ntHash, Is.EqualTo(userInfo?.NtHash));

                Assert.That(defaultPassword, Is.EqualTo(userInfo?.Password));
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
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
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
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
            }


            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator");

            if (userInfo is not null)
            {
                Assert.That(lmHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(ntHash, Is.EqualTo(userInfo?.NtHash));
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
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
            }


            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator");

            if (userInfo is not null)
            {
                Assert.That(lmHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(ntHash, Is.EqualTo(userInfo?.NtHash));
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
    public void LoadValidNTDS_Dump_WithCrack_OUFilter()
    {
        try
        {
            NtdsAuditor ntds = new(TestHelpers.NtdsPath, true, false, TestHelpers.SystemHivePath, TestHelpers.WordlistPath, TestHelpers.OuFilterPath);
            // Check we can actually process the ntds.dit file
            using (Assert.EnterMultipleScope())
            {
                Assert.That(domainCount, Is.EqualTo(ntds.Domains.Length));
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
            }


            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator");

            if (userInfo is not null)
            {
                Assert.That(lmHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(ntHash, Is.EqualTo(userInfo?.NtHash));

                Assert.That(defaultPassword, Is.EqualTo(userInfo?.Password));
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
                Assert.That(userCount, Is.EqualTo(ntds.Users.Length));
                Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
                Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
                Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Length));
                Assert.That(inactiveuser1YearCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 365)));
                Assert.That(inactiveuser90DaysCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.LastLogon) > 90)));
                Assert.That(userNoPasswordCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNotRequired)));
                Assert.That(userNonExpiryCount, Is.EqualTo(GetActiveUsers(ntds).Count(x => x.PasswordNeverExpires)));
                Assert.That(userOldPassword1Year, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 365)));
                Assert.That(userOldPassword90Days, Is.EqualTo(GetActiveUsers(ntds).Count(x => GetAge(x.PasswordLastChanged) > 90)));
                Assert.That(administratorCount, Is.EqualTo(1));
                Assert.That(domainAdminCount, Is.EqualTo(1));
                Assert.That(enterpriseAdminCount, Is.EqualTo(1));
                Assert.That(computerCount, Is.EqualTo(101));
                Assert.That(disableComputerCount, Is.Zero);
                Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
                Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
            }


            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.LmHash != string.Empty)));
            Assert.That(userCount, Is.EqualTo(ntds.Users.Count(u => u.NtHash != String.Empty)));

            // All users have the same password, so this should work for all user accounts, but it DEFINITELY should work for the administrator account....
            UserInfo userInfo = ntds.Users.First(u => u.Name.ToLowerInvariant() == "administrator");

            if (userInfo is not null)
            {
                Assert.That(lmHash, Is.EqualTo(userInfo?.LmHash));
                Assert.That(ntHash, Is.EqualTo(userInfo?.NtHash));

                Assert.That(defaultPassword, Is.EqualTo(userInfo?.Password));
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
    #endregion
    #region Helpers
    private UserInfo[] GetActiveUsers(NtdsAuditor ntds)
    {
        return [.. ntds.Users.Where(x => !x.Disabled && (!x.Expires.HasValue || x.Expires!.Value > baseDateTime))];
    }
    private double GetAge(DateTime referenceDate)
    {
        return (baseDateTime - referenceDate).TotalDays;
    }
    #endregion
}