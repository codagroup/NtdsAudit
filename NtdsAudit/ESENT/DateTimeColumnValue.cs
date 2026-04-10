namespace CODA.NtdsAudit.ESENT;

/// <summary>
/// A mockup replacement for the ESENT Interop DateTimeColumnValue data transfer object
/// </summary>
internal class DateTimeColumnValue : ColumnValueOfType<DateTime>
{
    #region Properties
    /// <summary>
    /// Gets or sets the column data value
    /// </summary>
    new public DateTime? Value { get; set; }

    protected override int Size => 0; // Internal JetDB function that returns 0 if either the value is null, or if the data type has a variable length.
    #endregion
    #region Functions
    /// <summary>
    /// Returns the value as a byte[].
    /// </summary>
    /// <returns></returns>
    public byte[]? GetValueAsBytes()
    {
        if (Value is null)
        {
            return null;
        }
        else
        {
            double oaDate = Value.Value.ToOADate(); //Handle OLE Automation Date format
            long ticks = BitConverter.DoubleToInt64Bits(oaDate);
            byte[] dateBytes = BitConverter.GetBytes(ticks);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(dateBytes);
            }
            return dateBytes;
        }
    }
    /// <summary>
    /// Converts a byte[] to its DateTimeColumnnValue equivalent. Failures result in the default date of 1899-12-30
    /// </summary>
    protected override void GetValueFromBytes(byte[] value, int startIndex = 0, int count = 0)
    {
        if (value is null || value.Length == 0)
        {
            Value = null;
        }
        else
        {
            long ticks = 0;
            byte[] dateBytes = (byte[])value[startIndex..(startIndex + count)].Clone();

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(dateBytes);
            }
            ticks = BitConverter.ToInt64(dateBytes, 0);
            double oaDate = BitConverter.Int64BitsToDouble(ticks);

            try
            {
                Value = DateTime.FromOADate(oaDate);
            }
            catch (ArgumentException)
            {
                Value = null;
            }
        }
    }
     #endregion
    #region Overrides
    public override string ToString()
    {
        return Value?.ToString("u") ?? string.Empty; //Return in ISO 8601 format or an empty string
    }
    #endregion
}
