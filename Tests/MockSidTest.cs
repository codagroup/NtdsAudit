#pragma warning disable CA1416

using System.Security.Principal;


namespace Tests
{
    [TestFixture]
    internal class MockSidTest
    {
        #region Fields
        private const string domainSid = "S-1-5-21-1999943590-2734198879-4288172524";
        private byte[] domainBytes = { 1, 4, 0, 0, 0, 0, 0, 5, 21, 0, 0, 0, 166, 183, 52, 119, 95, 144, 248, 162, 236, 81, 152, 255 };
        private const string adminSid = $"{domainSid}-500";
        private const string userSid = $"{domainSid}-1022";
        private const string nullSid = $"S-1-0-0";
        private const string systemSid = $"S-1-15-18";
        #endregion
        [Test]
        public void ValidateSids()
        {
            foreach (MockSidType mockSidType in Enum.GetValues(typeof(MockSidType)))
            {
                if (mockSidType == MockSidType.LogonIdsSid)
                {
                    try
                    {
                        Assert.Throws<ArgumentException>(() => new MockSid(mockSidType, null));
                    }
                    catch { }
                }
                else if (IsDomainSid(mockSidType))
                {
                    Assert.Throws<ArgumentNullException>(() => new MockSid(mockSidType, null));

                    MockSid mockDomainSid = new MockSid(domainSid);
                    MockSid mockSid = new MockSid(mockSidType, mockDomainSid);
                    Assert.That(mockSid.AccountDomainSid, Is.Not.Null);
                    if (mockSid.AccountDomainSid is not null)
                    {
                        try
                        {
                            Assert.That(mockSid.AccountDomainSid.ToString(), Is.EqualTo(domainSid));
                        }
                        catch { }
                    }
                }
                else
                {
                    MockSid mockSid = new MockSid(mockSidType, null);
                    SecurityIdentifier sid = new SecurityIdentifier(mockSid.SddlForm);
                    Assert.That(sid.ToString(), Is.EqualTo(mockSid.ToString()));
                }
            }
        }

        [Test]
        public void TestCreateSidFromString()
        {
            MockSid sid = new MockSid(nullSid);
            Assert.That(sid.ToString(), Is.EqualTo(nullSid));
            Assert.That(sid.SddlForm, Is.EqualTo(nullSid));

            sid = new MockSid(systemSid);
            Assert.That(sid.ToString(), Is.EqualTo(systemSid));
            Assert.That(sid.SddlForm, Is.EqualTo(systemSid));

            sid = new MockSid(userSid);
            Assert.That(sid.ToString(), Is.EqualTo(userSid));
            Assert.That(sid.SddlForm, Is.EqualTo(userSid));
            Assert.That(sid.AccountDomainSid, Is.Not.Null);
            if (sid.AccountDomainSid is not null)
            {
                Assert.That(sid.AccountDomainSid.ToString(), Is.EqualTo(domainSid));
            }

            sid = new MockSid(adminSid);
            Assert.That(sid.ToString(), Is.EqualTo(adminSid));
            Assert.That(sid.SddlForm, Is.EqualTo(adminSid));
            Assert.That(sid.AccountDomainSid, Is.Not.Null);
            if (sid.AccountDomainSid is not null)
            {
                Assert.That(sid.AccountDomainSid.ToString(), Is.EqualTo(domainSid));
            }
        }
        private bool IsDomainSid(MockSidType sidType)
        {
            switch (sidType)
            {
                case MockSidType.DomainAdministratorSid:
                case MockSidType.DomainGuestSid:
                case MockSidType.DomainKrbtgtSid:
                case MockSidType.DomainAdminsSid:
                case MockSidType.DomainUsersSid:
                case MockSidType.DomainGuestsSid:
                case MockSidType.DomainComputersSid:
                case MockSidType.DomainControllersSid:
                case MockSidType.DomainCertAdminsSid:
                case MockSidType.SchemaAdminsSid:
                case MockSidType.EnterpriseAdminsSid:
                case MockSidType.DomainPolicyAdminsSid:
                case MockSidType.DomainRasAndIasServersSid:
                case MockSidType.WinEnterpriseReadonlyControllersSid:
                case MockSidType.WinDomainReadonlyControllersSid:
                case MockSidType.WinEnterpriseReadonlyControllerSid:
                    return true;
                default:
                    return false;
            }
        }
    }
}
