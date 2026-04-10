namespace CODA.NtdsAudit.ESENT;

/// <summary>
/// A mockup representation of <see href="https://learn.microsoft.com/en-gb/windows/win32/extensible-storage-engine/attachdatabasegrbit-enumeration"/> without the core esent.dll dependency
/// </summary>
public enum AttachDatabaseGrbit
{
    None = 0,
    ReadOnly = 1,
    RecoveryOff = 8,
    DeleteCorruptIndexes = 0x10
}