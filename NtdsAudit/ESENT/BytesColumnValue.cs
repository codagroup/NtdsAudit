namespace CODA.NtdsAudit.ESENT;

internal class BytesColumnValue : ColumnValue
{
    #region Properties
    new public byte[] Value { get; set; } = [];
    public override int Length => Value.Length;
    protected override int Size => 0; // Internal JetDB function that returns 0 if either the value is null, or if the data type has a variable length.
    #endregion
    #region Overrides
    protected override void GetValueFromBytes(byte[] value, int startIndex, int count)
    {
        Value = new byte[count];
        Array.Copy(value, startIndex, Value, 0, count);
    }
    #endregion
}