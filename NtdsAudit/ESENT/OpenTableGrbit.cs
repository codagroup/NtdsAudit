namespace CODA.NtdsAudit.ESENT;

/// <summary>
/// A mockup representation of <see href="https://learn.microsoft.com/en-gb/windows/win32/extensible-storage-engine/opentablegrbit-enumeration"/> without the core esent.dll dependency
/// </summary>
[Flags]
public enum OpenTableGrbit
{
    None = 0,
    DenyWrite = 1,
    DenyRead = 2,
    ReadOnly = 4,
    Updatable = 8,
    PermitDDL = 0x10,
    NoCache = 0x20,
    Preread = 0x40,
    Sequential = 0x8000,
    TableClass1 = 0x10000,
    TableClass2 = 0x20000,
    TableClass3 = 0x30000,
    TableClass4 = 0x40000,
    TableClass5 = 0x50000,
    TableClass6 = 0x60000,
    TableClass7 = 0x70000,
    TableClass8 = 0x80000,
    TableClass9 = 0x90000,
    TableClass10 = 0xA0000,
    TableClass11 = 0xB0000,
    TableClass12 = 0xC0000,
    TableClass13 = 0xD0000,
    TableClass14 = 0xE0000,
    TableClass15 = 0xF0000
}
