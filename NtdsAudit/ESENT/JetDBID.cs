#region Usings
using System.Globalization;
#endregion

namespace CODA.NtdsAudit.ESENT;
/// <summary>
/// A mockup representation of <see href="https://learn.microsoft.com/en-gb/windows/win32/extensible-storage-engine/jet-dbid"/> without the core esent.dll dependency
/// </summary>
public struct JetDBID : IEquatable<JetDBID>, IFormattable
{
    #region Constructor
    public static JetDBID Nil => new()
    {
        Value = uint.MaxValue
    };
    #endregion
    #region Properties
    internal uint Value;
    #endregion
    #region Operators
    public static bool operator ==(JetDBID a, JetDBID b)
    {
        return a.Value == b.Value;
    }
    public static bool operator !=(JetDBID a, JetDBID b)
    {
        return !(a == b);
    }
    #endregion
    #region Overrides
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "JET_DBID({0})", Value); //Left as the original data type
    }
    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (!string.IsNullOrEmpty(format) && !("G" == format))
        {
            return Value.ToString(format, formatProvider);
        }
        else
        {
            return ToString();
        }
    }
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return Equals((JetDBID)obj);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    public bool Equals(JetDBID other)
    {
        return Value.Equals(other.Value);
    }
    #endregion
}