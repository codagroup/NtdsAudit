namespace CODA.NtdsAudit.ESENT;

/// <summary>
/// A mockup replacement for the ESENT Interop BoolColumnValue data transfer object
/// </summary>
internal class BoolColumnValue : ColumnValueOfType<bool>
{
    #region Properties
    new public bool? Value { get; set; }
    protected override int Size => sizeof(bool);
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
            byte[] dataBytes = BitConverter.GetBytes(Value.Value); // This feels a bit OTT for a bool, but JetDB does some strange things.
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(dataBytes);
            }
            return dataBytes;
        }
    }
    #endregion
    #region Overrides
    protected override void GetValueFromBytes(byte[] value, int startIndex=0, int count=0)
    {
        if (value is null || value.Length == 0)
        {
            Value = null;
        }
        else if (count != sizeof(bool))
        {
            throw new ArgumentException("Invalid data type provided");
        }
        else
        {
            byte[] dataBytes = (byte[])value[startIndex..(startIndex + count)].Clone(); // Again, it's a bool....
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(dataBytes);
            }
            Value = BitConverter.ToBoolean(dataBytes, 0);
        }
    }
    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
    #endregion
}
