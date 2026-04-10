using System;
using System.Collections.Generic;
using System.Text;

namespace CODA.NtdsAudit.ESENT;

[Flags]
public enum OpenDatabaseGrbit
{
    None = 0,
    ReadOnly = 1,
    Exclusive = 2
}
