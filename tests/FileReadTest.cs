#region Usings
#endregion

namespace Tests;

[TestFixture]
public class NTDSAudit_FileRead_Test
{
    #region Fields
    private int domainCount = 1;
    private int userCount = 103;
    private int activeUserCount = 87;
    private int disabledUserCount = 16;
    private int expiredUserCount = 0;
    private int inactiveuser1YearCount = 86;
    private int inactiveuser90DaysCount = 86;
    private int userNoPasswordCount = 0;
    private int userNonExpiryCount = 1;
    private int userOldPassword1Year = 0;
    private int userOldPassword90Days = 0;
    private int administratorCount = 1;
    private int domainAdminCount = 1;
    private int enterpriseAdminCount = 1;
    private int computerCount = 101;
    private int disableComputerCount = 0;
    private int inactiveComputer1YearCount = 100;
    private int inactiveComputer90DayCount = 100;
    private DateTime baseDateTime = new(2026, 01, 23, 14, 0, 32, DateTimeKind.Utc);
    #endregion
    #region Properties
    public string NtdsPath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}/testdata/ntds.dit";
    public string PwdumpPath { get; set; } = $"{Directory.GetCurrentDirectory()}/ad.lst";
    private string SystemHivePath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}/testdata/SYSTEM";
    private string WordlistPath { get; set; } = "";
    private string OuFilterPath { get; set; } = "";
    #endregion
    #region Tests
    [Test]
    public void LoadValidNTDS_NoDump()
    {
        try
        {
            NtdsAuditor ntds = new(NtdsPath, false, false, SystemHivePath, WordlistPath, OuFilterPath);
            Assert.That(domainCount, Is.EqualTo(ntds.Domains.Count()));
             Assert.That(userCount, Is.EqualTo(ntds.Users.Count()));
             Assert.That(disabledUserCount, Is.EqualTo(ntds.Users.Count(x => x.Disabled)));
             Assert.That(expiredUserCount, Is.EqualTo(ntds.Users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires!.Value < baseDateTime)));
             Assert.That(activeUserCount, Is.EqualTo(GetActiveUsers(ntds).Count()));
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
             Assert.That(disableComputerCount, Is.EqualTo(0));
             Assert.That(inactiveComputer1YearCount, Is.EqualTo(100));
             Assert.That(inactiveComputer90DayCount, Is.EqualTo(100));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [Test]
    public void LoadValidNTDS_Dump()
    {

    }

    [Test]
    public void LoadValidNTDS_Dump_WithHistory()
    {
        //TODO: Need to create users with history hashes.
    }

    [Test]
    public void LoadValidNTDS_Dump_WithCrack()
    {

    }

    [Test]
    public void LoadValidNTDS_Dump_WithHistory_WithCrack()
    {
        //TODO: Need to create users with history hashes.
    }
    [Test]
    public void LoadValidNTDS_NoDump_OUFilter()
    { }

    [Test]
    public void LoadValidNTDS_Dump_OUFilter()
    { }

    [Test]
    public void LoadValidNTDS_Dump_WithHistory_OUFilter()
    {
        //TODO: Need to create users with history hashes.
    }

    [Test]
    public void LoadValidNTDS_Dump_WithCrack_OUFilter()
    {

    }

    [Test]
    public void LoadValidNTDS_Dump_WithHistory_WithCrack_OUFilter()
    {
        //TODO: Need to create users with history hashes.
    }
    #endregion
    #region Helpers
    internal UserInfo[] GetActiveUsers(NtdsAuditor ntds)
    {
        return [.. ntds.Users.Where(x => !x.Disabled && (!x.Expires.HasValue || x.Expires!.Value > baseDateTime))];
    }
    internal double GetAge(DateTime referenceDate)
    {
        return (baseDateTime - referenceDate).TotalDays;
    }
    #endregion
}