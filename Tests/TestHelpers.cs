namespace Tests
{
    internal static class TestHelpers
    {
        internal static string NtdsPath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName}/testdata/ntds.dit";
        internal static string PwdumpPath { get; set; } = $"{Directory.GetCurrentDirectory()}/ad.lst";
        internal static string SystemHivePath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName}/testdata/SYSTEM";
        internal static string WordlistPath { get; set; } = $"{Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName}/testdata/wordlist.txt";
        internal static string OuFilterPath { get; set; } = "";
    }
}
