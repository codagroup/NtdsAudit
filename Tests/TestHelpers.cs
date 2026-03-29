namespace Tests
{
    internal static class TestHelpers
    {
        internal static string NtdsPath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName}/testdata/ntds.dit";
        internal static string PwdumpPath { get; set; } = $"{Directory.GetCurrentDirectory()}/ad.lst";
        internal static string SystemHivePath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName}/testdata/SYSTEM";
        internal static string WordlistPath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName}/testdata/wordlist.txt";
        internal static string OuFilterPath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName}/testdata/oufilters.txt";
        internal static DateTime BaseDateTime = new(2026, 01, 23, 14, 0, 32, DateTimeKind.Utc);
        internal static string LMHash = "AAD3B435B51404EEAAD3B435B51404EE"; //Empty LM Hash.
        internal static string NTHash = "DAE57D78FEC919471799CE0FAE8236B9"; //Pa55w0rd!
        internal static string DefaultPassword = "Pa55w0rd!";
    }
}
