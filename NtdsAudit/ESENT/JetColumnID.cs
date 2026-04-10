#region Usings
using System.Globalization;
#endregion

namespace CODA.NtdsAudit.ESENT;
/// <summary>
/// A mockup representation of <see href="https://learn.microsoft.com/en-gb/windows/win32/extensible-storage-engine/jet-columnid"/> without the core esent.dll dependency
/// </summary>
public struct JetColumnID : IEquatable<JetColumnID>, IComparable<JetColumnID>, IFormattable
{
    #region Constructor
    public static JetColumnID Nil => default;
    #endregion
    #region Properties
    internal uint Value;
    public bool IsInvalid
    {
        get
        {
            if (Value == 0)
            {
                return true;
            }
            else
            {
                return Value == uint.MaxValue;
            }
        }
    }
    #endregion
    #region Operators
    public static bool operator ==(JetColumnID a, JetColumnID b)
    {
        return a.Value == b.Value;
    }
    public static bool operator !=(JetColumnID a, JetColumnID b)
    {
        return !(a == b);
    }
    public static bool operator <(JetColumnID a, JetColumnID b)
    {
        return a.Value < b.Value;
    }

    public static bool operator >(JetColumnID a, JetColumnID b)
    {
        return a.Value > b.Value;
    }

    public static bool operator <=(JetColumnID a, JetColumnID b)
    {
        return a.Value <= b.Value;
    }

    public static bool operator >=(JetColumnID a, JetColumnID b)
    {
        return a.Value >= b.Value;
    }
    #endregion
    #region Overrides
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "JET_COLUMNID(0x{0:x})", Value); //Keep original datatype
    }
    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (!string.IsNullOrEmpty(format) && !("G" == format))
        {
            return Value.ToString(format, formatProvider);
        }
        return ToString();
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return Equals((JetColumnID)obj);
    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    public bool Equals(JetColumnID other)
    {
        return Value.Equals(other.Value);
    }
    public int CompareTo(JetColumnID other)
    {
        return Value.CompareTo(other.Value);
    }

    internal static JetColumnID CreateColumnidFromNativeValue(int nativeValue)
    {
        return new JetColumnID
        {
            Value = (uint)nativeValue
        };
    }
    #endregion
}