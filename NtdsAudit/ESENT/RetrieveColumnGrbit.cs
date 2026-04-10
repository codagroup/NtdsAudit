namespace CODA.NtdsAudit.ESENT;

public enum RetrieveColumnGrbit
{
    None = 0,
    RetrieveCopy = 1,
    RetrieveFromIndex = 2,
    RetrieveFromPrimaryBookmark = 4,
    RetrieveTag = 8,
    RetrieveNull = 0x10,
    RetrieveIgnoreDefault = 0x20
}