#region Usings
using System.Text;
#endregion

namespace CODA.NtdsAudit.ESENT;
/// <summary>
/// A mock up representation of <see href="https://learn.microsoft.com/en-us/windows/win32/extensible-storage-engine/api-class"/> without the esent.dll dependency.
/// </summary>
public static class Api
{
    #region Fields
    private static readonly Encoding _asciiEncoding;
    #endregion
    #region Events

    #endregion
    #region Handlers

    #endregion
    #region Constructor
    static Api() 
    {
        _asciiEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    }
    #endregion
    #region Properties

    #endregion
    #region Functions
    internal static IDictionary<string, JetColumnID> GetColumnDictionary(JetSesID sesid, JetTableId tableid)
    {
        /*
        JetGetTableColumnInfo(sesid, tableid, string.Empty, out JetColumnList list);
        Encoding encoding = _asciiEncoding; //Might need to be Unicode ..
        try
        {
            Dictionary<string, JetColumnID> dictionary = new Dictionary<string, JetColumnID>(list.ColumnRecord, StringComparer.OrdinalIgnoreCase); //ESE is apparently case-insensitive
            if (list.ColumnRecord > 0 && TryMoveFirst(sesid, list.TableId))
            {
                do
                {
                    string columnString = RetrieveColumnAsString(sesid, list.TableId, list.ColumnName, encoding, RetrieveColumnGrbit.None);
                    columnString = string.IsInterned(columnString) ?? columnString;
                    uint columnUint = RetrieveColumnAsUInt32(sesid, list.TableId, list.ColumnId).Value;
                    JetColumnID columnID = new JetColumnID()
                    {
                        Value = columnUint
                    };
                    dictionary.Add(columnString, columnID);
                } while (TryMoveNext(sesid, list.TableId));
            }
            return dictionary;
        }
        catch (Exception ex) 
        {
            //TODO
        }
        finally 
        {
            JetCloseTable(sesid, list.TableId);
        }
        */
        throw new NotImplementedException();
    }

    internal static void JetAttachDatabase(JetSesID sesid, string dbPath, AttachDatabaseGrbit bit = AttachDatabaseGrbit.ReadOnly)
    {
        throw new NotImplementedException();
        /*
         * TraceFunctionCall("JetAttachDatabase");
        CheckNotNull(database, "database");
        if (Capabilities.SupportsUnicodePaths)
        {
            return Err(NativeMethods.JetAttachDatabaseW(sesid.Value, database, (uint)grbit));
        }

        return Err(NativeMethods.JetAttachDatabaseA(sesid.Value, database, (uint)grbit));
         */
    }

    internal static void JetBeginSession(JetInstance instance, out JetSesID sesid, string username, string password)
    {
        throw new NotImplementedException();
        /*
         * TraceFunctionCall("JetBeginSession");
        sesid = JET_SESID.Nil;
        return Err(NativeMethods.JetBeginSessionA(instance.Value, out sesid.Value, username, password));
         */
    }

    internal static void JetCloseTable(JetSesID sesid, JetTableId tableid)
    {
        throw new NotImplementedException();
    }

    internal static void JetCreateInstance(out JetInstance instance, string name)
    {
        throw new NotImplementedException();
    }

    internal static void JetEndSession(JetSesID sesid, EndSessionGrbit bit = EndSessionGrbit.None)
    {
        throw new NotImplementedException();
    }

    internal static void JetGetDatabaseFileInfo(string dbPath, out int pageSize, JetDBInfo info = JetDBInfo.PageSize)
    {
        throw new NotImplementedException();
    }

    internal static void JetGetTableColumnInfo(JetSesID sesid, JetTableId tableid, string columnName, out JetColumnList list) 
    {
        JetGetTableColumnInfo(sesid, tableid, columnName, ColInfoGrbit.None, out list);
    }

    internal static void JetGetTableColumnInfo(JetSesID sesId, JetTableId tableId, string columnName, ColInfoGrbit bit, out JetColumnList columnlist)
    { 
        throw new NotImplementedException();
    }
    internal static void JetInit(ref JetInstance instance)
    {
        throw new NotImplementedException();
    }

    internal static void JetOpenDatabase(JetSesID sesid, string dbPath, string connect, out JetDBID dbid, OpenDatabaseGrbit bit = OpenDatabaseGrbit.ReadOnly)
    {
        throw new NotImplementedException();
    }

    internal static void JetOpenTable(JetSesID sesid, JetDBID dbid, string tablename, byte[]? parameters, int parameterLength, OpenTableGrbit bit, out JetTableId tableid)
    {
        throw new NotImplementedException();
    }

    internal static void JetSetSystemParameter(JetInstance instance, JetSesID sesID, JetParameter jetParameter, int paramInt, string paramString)
    {
        if (paramInt < 0) //Nullable?
        {

        }
        if (paramString == string.Empty) //Nullable?
        {

        }
        throw new NotImplementedException();
    }

    internal static void JetTerm(JetInstance instance)
    {
        throw new NotImplementedException();
    }

    internal static void MoveBeforeFirst(JetSesID sesid, JetTableId tableid)
    {
        throw new NotImplementedException();
    }

    internal static void RetrieveColumns(JetSesID sesid, JetTableId tableid, ColumnValue[] values)
    {
        throw new NotImplementedException();
    }
    internal static string RetrieveColumnAsString(JetSesID sesId, JetTableId tableId, JetColumnID columnName, Encoding encoding, RetrieveColumnGrbit bit = RetrieveColumnGrbit.None)
    {
        throw new NotImplementedException();
    }
    internal static bool TryMoveFirst(JetSesID sesId, JetTableId tableId)
    {
        throw new NotImplementedException();
    }

    internal static bool TryMoveNext(JetSesID sesid, JetTableId tableid)
    {
        throw new NotImplementedException();
    }
    #endregion
}
