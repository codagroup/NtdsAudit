#pragma warning disable CA1416

using System.Security.Principal;


namespace Tests
{
    [TestFixture]
    internal class MockSidTest
    {
        #region Fields
        private const string domainSid = "S-1-5-21-1999943590-2734198879-4288172524";
        private byte[] domainBytes = {1,4,0,0,0,0,0,5,21,0,0,0,166,183,52,119,95,144,248,162,236,81,152,255};
        private const string adminSid = $"{domainSid}-500";
        private const string userSid = $"{domainSid}-1022";
        #endregion
        [Test]
        public void ValidateSids()
        {
            foreach (MockSidType mockSidType in Enum.GetValues(typeof(MockSidType)))
            {
                try
                {
                    MockSid mockSid = new MockSid(mockSidType, null);
                    SecurityIdentifier sid = new SecurityIdentifier(mockSid.SddlForm);
                    Assert.That(sid.ToString(), Is.EqualTo(mockSid.ToString()));
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
        }
    }
}
