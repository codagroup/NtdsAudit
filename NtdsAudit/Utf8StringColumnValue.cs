namespace CODA.NtdsAudit;

using CODA.NtdsAudit.ESENT;
using System.Text;

/// <summary>
/// A UTF8 string column value.
/// </summary>
public class Utf8StringColumnValue : StringColumnValue
{
    /// <inheritdoc/>
    public override void SetValueFromBytes(byte[] value, int startIndex, int count, int err)
    {
        Value = Encoding.UTF8.GetString(value, startIndex, count);
    }
}