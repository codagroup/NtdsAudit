#region Usings
using System.CommandLine;
using System.Diagnostics;
#endregion

namespace NtdsAudit;

public static class Program
{
    #region Constructors
    public static int Main(string[] args)
    {
        RootCommand cmd = new("A utility for auditing Active Directory");
        Argument<FileInfo> ntdsPath = new("NTDS file") { Description = "The path of the NTDS.dit database to be audited, required." };
        Option<FileInfo> systemHivePath = new("-s", "--system") { Description = "The path of the associated SYSTEM hive. Required when using the pwdump option.", Arity = ArgumentArity.ExactlyOne, HelpName = "path" };
        Option<FileInfo> pwdumpPath = new("-p", "--pwdump") { Description = "The path to output hashes in pwdump format.", Arity = ArgumentArity.ExactlyOne, HelpName = "path" };
        Option<FileInfo> usersCsvPath = new("-u", "--users-csv") { Description = "The path to output user details in CSV format.", Arity = ArgumentArity.ExactlyOne, HelpName = "path" };
        Option<FileInfo> computersCsvPath = new("-c", "--computers-csv") { Description = "The path to output computer details in CSV format.", Arity = ArgumentArity.ExactlyOne, HelpName = "path" };
        Option<bool> anonymise = new("--anonymise") { Description = "Anonymise the pwdump and reversible passwords files.", Arity = ArgumentArity.ZeroOrOne };
        Option<bool> includeHistoryHashes = new("--history-hashes") { Description = "Include history hashes in the pdwump output.", Arity = ArgumentArity.ZeroOrOne };
        Option<FileInfo> dumpReversiblePath = new("--dump-reversible") { Description = "The path to output clear text passwords if reversible encryption is enabled.", Arity = ArgumentArity.ExactlyOne, HelpName = "path" };
        Option<FileInfo> wordlistPath = new("--wordlist") { Description = "The path to a wordlist of weak passwords for basic hash cracking. Warning: Using this option is slow. The use of a dedicated password cracker such as 'john' is recommended instead.", Arity = ArgumentArity.ExactlyOne, HelpName = "path" };
        Option<FileInfo> ouFilterFilePath = new("--ou-filter-file") { Description = "The path to file containing a line separated list of OUs to which to limit user and computer results.", Arity = ArgumentArity.ExactlyOne, HelpName = "path" };
        Option<DateOnly> baseDate = new("--base-date") { Description = "Specifies a custom date to be used as the base date in statistics. The last modified date of the NTDS file is used by default.", Arity = ArgumentArity.ExactlyOne, HelpName = "date" };
        Option<bool> useRdn = new("--useRdn") { Description = "Refer to users via name attribute (RDN) rather than samAccountName attribute in outputs.", Arity = ArgumentArity.ZeroOrOne };
        Option<bool> debug = new("--debug") { Description = "Show debug output.", Arity = ArgumentArity.ZeroOrOne };

        cmd.Arguments.Add(ntdsPath);
        cmd.Options.Add(systemHivePath);
        cmd.Options.Add(pwdumpPath);
        cmd.Options.Add(usersCsvPath);
        cmd.Options.Add(computersCsvPath);
        cmd.Options.Add(anonymise);
        cmd.Options.Add(includeHistoryHashes);
        cmd.Options.Add(dumpReversiblePath);
        cmd.Options.Add(wordlistPath);
        cmd.Options.Add(ouFilterFilePath);
        cmd.Options.Add(baseDate);
        cmd.Options.Add(useRdn);
        cmd.Options.Add(debug);

        cmd.Validators.Add(result =>
        {
            if (result.GetValue(ntdsPath) is not null && !result.GetValue(ntdsPath)!.Exists)
            {
                result.AddError($"The path provided for the NTDS file ({ntdsPath.Name}) was not valid.");
            }
            if (result.GetValue(systemHivePath) is not null && !result.GetValue(systemHivePath)!.Exists)
            {
                result.AddError($"The path provided for the SYSTEM hive ({systemHivePath.Name}) was not valid.");
            }
            if (result.GetValue(pwdumpPath) is not null && result.GetValue(systemHivePath) is null)
            {
                result.AddError($"The path of the associated SYSTEM hive ({systemHivePath.Name}) is required when the {pwdumpPath.Name} option is set.");
            }
        });

        ParseResult result = cmd.Parse(args);
        if (!result.Errors.Any())
        {
            NtdsAudit ntds = new(
                result.GetValue(ntdsPath)!.FullName,
                result.GetValue(pwdumpPath) is not null,
                result.GetValue(includeHistoryHashes),
                result.GetValue(systemHivePath) is not null ? result.GetValue(systemHivePath)!.FullName : "",
                result.GetValue(wordlistPath) is not null ? result.GetValue(wordlistPath)!.FullName : "",
                result.GetValue(ouFilterFilePath) is not null ? result.GetValue(ouFilterFilePath)!.FullName : ""
            );
            
            PrintConsoleStatistics(
                ntds,
                result.GetValue(baseDate) == DateOnly.FromDayNumber(0) ? result.GetValue(ntdsPath)!.LastWriteTimeUtc : result.GetValue(baseDate).ToDateTime(new(),DateTimeKind.Utc)
            );

            if (result.GetValue(pwdumpPath) is not null)
            {
                WritePwDumpFile(
                    result.GetValue(pwdumpPath)!.FullName,
                    ntds,
                    result.GetValue(baseDate) == DateOnly.FromDayNumber(0) ? result.GetValue(ntdsPath)!.LastWriteTimeUtc : result.GetValue(baseDate).ToDateTime(new(),DateTimeKind.Utc),
                    result.GetValue(includeHistoryHashes),
                    result.GetValue(wordlistPath) is not null,
                    result.GetValue(dumpReversiblePath) is not null ? result.GetValue(dumpReversiblePath)!.FullName : "",
                    result.GetValue(useRdn),
                    result.GetValue(anonymise)
                );
            }

            if (result.GetValue(usersCsvPath) is not null)
            {
                WriteUsersCsvFile(
                    result.GetValue(usersCsvPath)!.FullName,
                    ntds,
                    result.GetValue(baseDate) == DateOnly.FromDayNumber(0) ? result.GetValue(ntdsPath)!.LastWriteTimeUtc : result.GetValue(baseDate).ToDateTime(new(),DateTimeKind.Utc),
                    result.GetValue(useRdn)
                );
            }

            if (result.GetValue(computersCsvPath) is not null)
            {
                WriteComputersCsvFile(
                    result.GetValue(computersCsvPath)!.FullName,
                    ntds,
                    result.GetValue(baseDate) == DateOnly.FromDayNumber(0) ? result.GetValue(ntdsPath)!.LastWriteTimeUtc : result.GetValue(baseDate).ToDateTime(new(),DateTimeKind.Utc)
                );
            }
        }
        return result.Invoke();
    }
    #endregion
    #region Functions
    [Conditional("DEBUG")]
    private static void LaunchDebugger()
    {
        if (!Debugger.IsAttached) { Debugger.Launch(); }
    }
    private static void PrintConsoleStatistics(NtdsAudit ntds, DateTime baseDateTime)
    {
        Console.WriteLine();
        Console.WriteLine($"The base date used for statistics is {baseDateTime}");
        Console.WriteLine();

        foreach (var domain in ntds.Domains)
        {
            Console.WriteLine($"Account stats for: {domain.Fqdn}");

            var users = ntds.Users.Where(x => x.DomainSid.Equals(domain.Sid)).ToList();
            var totalUsersCount = users.Count;
            var disabledUsersCount = users.Count(x => x.Disabled);
            var expiredUsersCount = users.Count(x => !x.Disabled && x.Expires.HasValue && x.Expires.Value < baseDateTime);
            var activeUsers = users.Where(x => !x.Disabled && (!x.Expires.HasValue || x.Expires.Value > baseDateTime)).ToList();
            var activeUsersCount = activeUsers.Count;
            var activeUsersUnusedIn1Year = activeUsers.Count(x => x.LastLogon + TimeSpan.FromDays(365) < baseDateTime);
            var activeUsersUnusedIn90Days = activeUsers.Count(x => x.LastLogon + TimeSpan.FromDays(90) < baseDateTime);
            var activeUsersWithPasswordNotRequired = activeUsers.Count(x => x.PasswordNotRequired);
            var activeUsersWithPasswordNeverExpires = activeUsers.Count(x => x.PasswordNeverExpires);
            var activeUsersPasswordUnchangedIn1Year = activeUsers.Count(x => x.PasswordLastChanged + TimeSpan.FromDays(365) < baseDateTime);
            var activeUsersPasswordUnchangedIn90Days = activeUsers.Count(x => x.PasswordLastChanged + TimeSpan.FromDays(90) < baseDateTime);

            var activeUsersWithAdministratorMembership = activeUsers.Where(x => x.RecursiveGroupSids.Contains(domain.AdministratorsSid)).ToArray();
            var activeUsersWithDomainAdminMembership = activeUsers.Where(x => x.RecursiveGroupSids.Contains(domain.DomainAdminsSid)).ToArray();

            // Unlike Domain Admins and Adminsitrators, Enterprise Admins is not domain local, so include all users.
            var activeUsersWithEnterpriseAdminMembership = ntds.Users.Where(x => !x.Disabled && (!x.Expires.HasValue || x.Expires.Value > baseDateTime) && x.RecursiveGroupSids.Contains(domain.EnterpriseAdminsSid)).ToArray();

            WriteStatistic("Disabled users", disabledUsersCount, totalUsersCount);
            WriteStatistic("Expired users", expiredUsersCount, totalUsersCount);
            WriteStatistic("Active users unused in 1 year", activeUsersUnusedIn1Year, activeUsersCount);
            WriteStatistic("Active users unused in 90 days", activeUsersUnusedIn90Days, activeUsersCount);
            WriteStatistic("Active users which do not require a password", activeUsersWithPasswordNotRequired, activeUsersCount);
            WriteStatistic("Active users with non-expiring passwords", activeUsersWithPasswordNeverExpires, activeUsersCount);
            WriteStatistic("Active users with password unchanged in 1 year", activeUsersPasswordUnchangedIn1Year, activeUsersCount);
            WriteStatistic("Active users with password unchanged in 90 days", activeUsersPasswordUnchangedIn90Days, activeUsersCount);
            WriteStatistic("Active users with Administrator rights", activeUsersWithAdministratorMembership.Length, activeUsersCount);
            WriteStatistic("Active users with Domain Admin rights", activeUsersWithDomainAdminMembership.Length, activeUsersCount);
            WriteStatistic("Active users with Enterprise Admin rights", activeUsersWithEnterpriseAdminMembership.Length, activeUsersCount);
            Console.WriteLine();

            var computers = ntds.Computers.Where(x => x.DomainSid.Equals(domain.Sid)).ToList();
            var totalComputersCount = computers.Count;
            var disabledComputersCount = computers.Count(x => x.Disabled);
            var activeComputers = computers.Where(x => !x.Disabled).ToList();
            var activeComputersCount = activeComputers.Count;
            var activeComputersUnusedIn1Year = activeComputers.Count(x => x.LastLogon + TimeSpan.FromDays(365) < baseDateTime);
            var activeComputersUnusedIn90Days = activeComputers.Count(x => x.LastLogon + TimeSpan.FromDays(90) < baseDateTime);

            WriteStatistic("Disabled computers", disabledComputersCount, totalComputersCount);
            WriteStatistic("Active computers unused in 1 year", activeComputersUnusedIn1Year, activeComputersCount);
            WriteStatistic("Active computers unused in 90 days", activeComputersUnusedIn90Days, activeComputersCount);
            Console.WriteLine();
        }
    }
    private static void WriteComputersCsvFile(string computersCsvPath, NtdsAudit ntdsAudit, DateTime baseDateTime)
    {
        using (var file = new StreamWriter(computersCsvPath, false))
        {
            file.WriteLine("Domain,Computer,Disabled,Last Logon,DN");
            foreach (var computer in ntdsAudit.Computers)
            {
                var domain = ntdsAudit.Domains.Single(x => x.Sid == computer.DomainSid);
                file.WriteLine($"{domain.Fqdn},{computer.Name},{computer.Disabled},{computer.LastLogon},\"{computer.Dn}\"");
            }
        }
    }
    private static void WritePwDumpFile(string pwdumpPath, NtdsAudit ntdsAudit, DateTime baseDateTime, bool includeHistoryHashes, bool wordlistInUse, string dumpReversiblePath, bool useRdn, bool anonymise)
    {
        DomainInfo domain = null;

        // NTDS will only contain hashes for a single domain, even when NTDS was dumped from a global catalog server, ensure we only print hashes for that domain, and warn the user if there are other domains in NTDS
        if (ntdsAudit.Domains.Length > 1)
        {
            var usersWithHashes = ntdsAudit.Users.Where(x => x.LmHash != NtdsAudit.EMPTY_LM_HASH || x.NtHash != NtdsAudit.EMPTY_NT_HASH).ToList();
            domain = ntdsAudit.Domains.Single(x => x.Sid.Equals(usersWithHashes[0].DomainSid));

            ConsoleEx.WriteWarning($"WARNING:");
            ConsoleEx.WriteWarning($"The NTDS file has been retrieved from a global catalog (GC) server. Whilst GCs store information for other domains, they only store password hashes for their primary domain.");
            ConsoleEx.WriteWarning($"Password hashes have only been dumped for the \"{domain.Fqdn}\" domain.");
            ConsoleEx.WriteWarning($"If you require password hashes for other domains, please obtain the NTDS and SYSTEM files for each domain.");
            Console.WriteLine();
        }
        else
        {
            domain = ntdsAudit.Domains[0];
        }

        var users = ntdsAudit.Users.Where(x => domain.Sid.Equals(x.DomainSid)).ToArray();

        if (users.Any(x => !string.IsNullOrEmpty(x.ClearTextPassword)))
        {
            ConsoleEx.WriteWarning($"WARNING:");
            ConsoleEx.WriteWarning($"The NTDS file contains user accounts with passwords stored using reversible encryption. Use the --dump-reversible option to output these users and passwords.");
            Console.WriteLine();
        }

        var activeUsers = users.Where(x => !x.Disabled && (!x.Expires.HasValue || x.Expires.Value > baseDateTime)).ToArray();
        var activeUsersWithLMs = activeUsers.Where(x => !string.IsNullOrEmpty(x.LmHash) && x.LmHash != NtdsAudit.EMPTY_LM_HASH).ToArray();
        var activeUsersWithWeakPasswords = activeUsers.Where(x => !string.IsNullOrEmpty(x.Password)).ToArray();
        var activeUsersWithDuplicatePasswordsCount = activeUsers.Where(x => x.NtHash != NtdsAudit.EMPTY_NT_HASH).GroupBy(x => x.NtHash).Where(g => g.Count() > 1).Sum(g => g.Count());
        var activeUsersWithPasswordStoredUsingReversibleEncryption = activeUsers.Where(x => !string.IsNullOrEmpty(x.ClearTextPassword)).ToArray();

        Console.WriteLine($"Password stats for: {domain.Fqdn}");
        WriteStatistic("Active users using LM hashing", activeUsersWithLMs.Length, activeUsers.Length);
        WriteStatistic("Active users with duplicate passwords", activeUsersWithDuplicatePasswordsCount, activeUsers.Length);
        WriteStatistic("Active users with password stored using reversible encryption", activeUsersWithPasswordStoredUsingReversibleEncryption.Length, activeUsers.Length);
        if (wordlistInUse)
        {
            WriteStatistic("Active user accounts with very weak passwords", activeUsersWithWeakPasswords.Length, activeUsers.Length);
        }

        Console.WriteLine();

        // <username>:<uid>:<LM-hash>:<NTLM-hash>:<comment>:<homedir>:
        using (var file = new StreamWriter(pwdumpPath, false))
        {
            StreamWriter mapFile = null;
            if (anonymise)
            {
                mapFile = new StreamWriter($"{pwdumpPath}.map", false);
            }

            var r = new Random((int)((DateTimeOffset)baseDateTime).ToUnixTimeSeconds());
            for (var i = 0; i < users.Length; i++)
            {
                string userId;

                if (anonymise)
                {
                    var bytes = new byte[16];
                    r.NextBytes(bytes);
                    userId = new Guid(bytes).ToString();
                }
                else if (useRdn)
                {
                    userId = users[i].Name;
                }
                else
                {
                    userId = users[i].SamAccountName;
                }

                var comments = $"Disabled={users[i].Disabled}," +
                    $"Expired={!users[i].Disabled && users[i].Expires.HasValue && users[i].Expires.Value < baseDateTime}," +
                    $"PasswordNeverExpires={users[i].PasswordNeverExpires}," +
                    $"PasswordNotRequired={users[i].PasswordNotRequired}," +
                    $"PasswordLastChanged={users[i].PasswordLastChanged.ToString("yyyyMMddHHmm")}," +
                    $"LastLogonTimestamp={users[i].LastLogon.ToString("yyyyMMddHHmm")}," +
                    $"IsAdministrator={users[i].RecursiveGroupSids.Contains(domain.AdministratorsSid)}," +
                    $"IsDomainAdmin={users[i].RecursiveGroupSids.Contains(domain.DomainAdminsSid)}," +
                    $"IsEnterpriseAdmin={users[i].RecursiveGroupSids.Intersect(ntdsAudit.Domains.Select(x => x.EnterpriseAdminsSid)).Any()}";
                var homeDir = string.Empty;
                file.Write($"{(anonymise ? string.Empty : $"{domain.Fqdn}\\")}{userId}:{(anonymise ? i : users[i].Rid)}:{users[i].LmHash}:{users[i].NtHash}:{comments}:{homeDir}:");

                if (includeHistoryHashes && users[i].NtHistory != null && users[i].NtHistory.Length > 0)
                {
                    file.Write(Environment.NewLine);
                }
                else if (i < users.Length - 1)
                {
                    file.Write(Environment.NewLine);
                }

                if (includeHistoryHashes && users[i].NtHistory != null && users[i].NtHistory.Length > 0)
                {
                    for (var j = 0; j < users[i].NtHistory.Length; j++)
                    {
                        var lmHash = (users[i].LmHistory?.Length > j) ? users[i].LmHistory[j] : NtdsAudit.EMPTY_LM_HASH;
                        file.Write($"{(anonymise ? string.Empty : $"{domain.Fqdn}\\")}{userId}__history_{j}:{(anonymise ? i : users[i].Rid)}:{lmHash}:{users[i].NtHistory[j]}:::");

                        if (j < users[i].NtHistory.Length || i < users.Length - 1)
                        {
                            file.Write(Environment.NewLine);
                        }
                    }
                }

                if (anonymise)
                {
                    mapFile.Write($"{userId}:{domain.Fqdn}\\{(useRdn ? users[i].Name : users[i].SamAccountName)}");
                    mapFile.Write(Environment.NewLine);
                }
            }

            if (anonymise)
            {
                mapFile.Dispose();
            }
        }

        if (users.Any(x => !string.IsNullOrEmpty(x.ClearTextPassword)) && !string.IsNullOrWhiteSpace(dumpReversiblePath))
        {
            using (var file = new StreamWriter(dumpReversiblePath, false))
            {
                var r = new Random((int)((DateTimeOffset)baseDateTime).ToUnixTimeSeconds());
                for (var i = 0; i < users.Length; i++)
                {
                    string userId;

                    if (anonymise)
                    {
                        var bytes = new byte[16];
                        r.NextBytes(bytes);
                        userId = new Guid(bytes).ToString();
                    }
                    else if (useRdn)
                    {
                        userId = users[i].Name;
                    }
                    else
                    {
                        userId = users[i].SamAccountName;
                    }

                    if (!string.IsNullOrEmpty(users[i].ClearTextPassword))
                    {
                        file.Write($"{domain.Fqdn}\\{userId}:{users[i].ClearTextPassword}");

                        if (i < users.Length - 1)
                        {
                            file.Write(Environment.NewLine);
                        }
                    }
                }
            }
        }
    }

    private static void WriteStatistic(string statistic, int actual, int maximum)
    {
        Console.Write($"  {statistic} ".PadRight(70, '_'));
        var percentageString = (maximum < 1) ? "N/A" : GetPercentage(actual, maximum) + "%";
        Console.Write($" {actual.ToString().PadLeft(5)} of {maximum.ToString().PadLeft(5)} ({percentageString})");
        Console.Write(Environment.NewLine);
    }

    private static void WriteUsersCsvFile(string usersCsvPath, NtdsAudit ntdsAudit, DateTime baseDateTime, bool useRdn)
    {
        using (var file = new StreamWriter(usersCsvPath, false))
        {
            file.WriteLine("Domain,Username,Administrator,Domain Admin,Enterprise Admin,Disabled,Expired,Password Never Expires,Password Not Required,Password Last Changed,Last Logon,DN");
            foreach (var user in ntdsAudit.Users)
            {
                var domain = ntdsAudit.Domains.Single(x => x.Sid == user.DomainSid);
                file.WriteLine($"{domain.Fqdn},{(useRdn ? user.Name : user.SamAccountName)},{user.RecursiveGroupSids.Contains(domain.AdministratorsSid)},{user.RecursiveGroupSids.Contains(domain.DomainAdminsSid)},{user.RecursiveGroupSids.Intersect(ntdsAudit.Domains.Select(x => x.EnterpriseAdminsSid)).Any()},{user.Disabled},{!user.Disabled && user.Expires.HasValue && user.Expires.Value < baseDateTime},{user.PasswordNeverExpires},{user.PasswordNotRequired},{user.PasswordLastChanged},{user.LastLogon},\"{user.Dn}\"");
            }
        }
    }

    private static double GetPercentage(int actual, int max)=> Math.Round(((double)100 / max) * actual, 1);
    #endregion
}