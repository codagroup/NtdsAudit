using System;
using System.Collections.Generic;
using System.Text;

namespace CODA.NtdsAudit.ESENT;

/// <summary>
/// A mockup representation of <see href="https://learn.microsoft.com/en-gb/windows/win32/extensible-storage-engine/jet-dbinfo-enumeration"/> without the core esent.dll dependency
/// </summary>
public enum JetDBInfo
{
    Filename = 0,
    LCID = 3,
    Options = 6,
    Transactions = 7,
    Version = 8,
    Filesize = 10,
    SpaceOwned = 11,
    SpaceAvailable = 12,
    Misc = 14,
    DBInUse = 15,
    PageSize = 17,
    FileType = 19
}
