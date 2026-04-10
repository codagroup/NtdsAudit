#region Usings
using System.Globalization;
#endregion

namespace CODA.NtdsAudit.ESENT;
/// <summary>
/// A mockup representation of <see href="https://learn.microsoft.com/en-us/windows/win32/extensible-storage-engine/jet-columnlist-class"/> without the core esent.dll dependency
/// </summary>
public class JetColumnList
{
    #region Properties
    public JetTableId TableId { get; set; }
    public int ColumnRecord { get; internal set;  }
    public JetColumnID ColumnName { get; internal set; }

    public JetColumnID ColumnId { get; internal set; }

    public JetColumnID ColumnType { get; internal set; }

    public JetColumnID CP { get; internal set; } //What is this for?

    public JetColumnID CbMax { get; internal set; } //What is this for?

    public JetColumnID Grbit { get; internal set; }

    public JetColumnID Default { get; internal set; }

    public JetColumnID BaseTableName { get; internal set; }
    #endregion
    #region Overrides
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "JET_COLUMNLIST(0x{0:x},{1} records)", TableId, ColumnRecord);
    }
    #endregion
    #region Functions
    internal void SetFromNativeColumnList(NativeColumnList list)
    {
        TableId = new JetTableId
        {
            Value = list.tableid
        };
        ColumnRecord = checked((int)list.cRecord);
        ColumnName = new JetColumnID
        {
            Value = list.columnidcolumnname
        };
        ColumnId = new JetColumnID
        {
            Value = list.columnidcolumnid
        };
        ColumnType = new JetColumnID
        {
            Value = list.columnidcoltyp
        };
        CP = new JetColumnID
        {
            Value = list.columnidCp
        };
        CbMax = new JetColumnID
        {
            Value = list.columnidcbMax
        };
        Grbit = new JetColumnID
        {
            Value = list.columnidgrbit
        };
        Default = new JetColumnID
        {
            Value = list.columnidDefault
        };
        BaseTableName = new JetColumnID
        {
            Value = list.columnidBaseTableName
        };
    }
    #endregion
}
internal struct NativeColumnList
{
    public uint cbStruct;
    public nint tableid;
    public uint cRecord;
    public uint columnidPresentationOrder;
    public uint columnidcolumnname;
    public uint columnidcolumnid;
    public uint columnidcoltyp;
    public uint columnidCountry;
    public uint columnidLangid;
    public uint columnidCp;
    public uint columnidCollate;
    public uint columnidcbMax;
    public uint columnidgrbit;
    public uint columnidDefault;
    public uint columnidBaseTableName;
    public uint columnidBaseColumnName;
    public uint columnidDefinitionName;
}