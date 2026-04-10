#region Usings
using System.Text;
#endregion

namespace CODA.NtdsAudit.ESENT;

/// <summary>
/// A mockup replacement for the ESENT Interop StringColumnValue data transfer object
/// </summary>
internal class StringColumnValue : ColumnValue
{
    #region Properties
    /// <summary>
    /// Gets or sets the column data value
    /// </summary>
    new public string Value { get; set; } = string.Empty;
    protected override int Size => 0; // Internal JetDB function that returns 0 if either the value is null, or if the data type has a variable length.
    public override int Length => Value.Length;
    #endregion
    #region Functions
    /// <summary>
    /// Returns the value as a byte[], or an empty array if the Value isn't set (i.e., it's an empty string).
    /// </summary>
    /// <returns></returns>
    public byte[]? GetValueAsBytes()
    {
        if (Value == string.Empty)
        {
            return null;
        }
        else
        {
            byte[] valueBytes = Encoding.Unicode.GetBytes(Value);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBytes);
            }
            return valueBytes;
        }
    }
    /// <summary>
    /// Convers a byte[] to its StringColumnValue equivalent. Note that this uses Unicode. Some varients, such as LDAP, require UTF8.
    /// </summary>
    protected override void GetValueFromBytes(byte[] value, int startIndex = 0, int count = 0)
    {
        Value = Encoding.Unicode.GetString(value, startIndex, count);
    }
    #endregion
    #region Overrides
    public override string ToString()
    {
        return Value;
    }
    #endregion
}
