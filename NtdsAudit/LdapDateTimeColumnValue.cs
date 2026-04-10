using CODA.NtdsAudit.ESENT;

namespace CODA.NtdsAudit;

/// <summary>
/// A date time column value based on the LDAP epoch.
/// </summary>
internal class LdapDateTimeColumnValue : DateTimeColumnValue
{
    /// <inheritdoc/>
    protected void GetValueFromBytes(byte[] value, int startIndex, int count, JetWarningType warning)
    {
        if (warning == JetWarningType.ColumnNull)
        {
            Value = null;
        }
        else
        {
            CheckDataCount(count);
            var ticks = BitConverter.ToInt64(value, startIndex);
            Value = new DateTime(1601, 1, 1).AddTicks(ticks);
        }
    }
}