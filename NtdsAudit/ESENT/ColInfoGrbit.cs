namespace CODA.NtdsAudit.ESENT;

[Flags]
public enum ColInfoGrbit
{
    None = 0,
    NonDerivedColumnsOnly = int.MinValue,
    MinimalInfo = 0x40000000,
    SortByColumnid = 0x20000000
}