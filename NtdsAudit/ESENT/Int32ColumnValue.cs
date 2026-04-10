namespace CODA.NtdsAudit.ESENT;

/// <summary>
/// A mockup replacement for the ESENT Interop Int32ColumnValue data transfer object
/// </summary>
internal class Int32ColumnValue : ColumnValueOfType<int>
{
    #region Properties
    new public int? Value { get; set; }
    protected override int Size => sizeof(int);
    #endregion
    #region Functions
    public byte[]? GetValueAsBytes()
    {
        if (Value is null)
        {
            return null;
        }
        else
        {
            byte[] dataBytes = BitConverter.GetBytes(Value.Value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(dataBytes);
            }
            return dataBytes;
        }
    }
    protected override void GetValueFromBytes(byte[] value, int startIndex = 0, int count = 0)
    {
        if (value is null || value.Length == 0) 
        {
               Value = null;
        }
        else if (count != sizeof(int))
        {
            throw new ArgumentException("Invalid data type provided");
        }
        else
        {
            byte[] dataBytes = (byte[])value[startIndex..(startIndex + count)].Clone();

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(dataBytes);
            }
            Value = BitConverter.ToInt32(dataBytes, 0);
        }
    }
    #endregion
    #region Overrides
    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
    #endregion
}
