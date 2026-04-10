#region Usings
using System.Globalization;
#endregion

namespace CODA.NtdsAudit.ESENT;
/// <summary>
/// A mockup representation of <see href="https://learn.microsoft.com/en-gb/windows/win32/extensible-storage-engine/jet-tableid"/> without the core esent.dll dependency
/// </summary>
public struct JetTableId : IEquatable<JetTableId>, IFormattable
{
    #region Constructor
    public static JetTableId Nil => default;
    #endregion
    #region Properties
    internal nint Value;
    public bool IsInvalid
    {
        get
        {
            if (Value == nint.Zero)
            {
                return true;
            }
            else
            {
                return Value == new nint(-1);
            }
        }
    }
    #endregion
    #region Operators
    public static bool operator ==(JetTableId a, JetTableId b)
    {
        return a.Value == b.Value;
    }
    public static bool operator !=(JetTableId a, JetTableId b)
    {
        return !(a == b);
    }
    #endregion
    #region Overrides
    public override string ToString()
    {
        return string.Format(CultureInfo.InvariantCulture, "JET_TABLEID(0x{0:x})", ((nint)Value).ToInt64()); //Keep original data type
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (!string.IsNullOrEmpty(format) && !("G" == format))
        {
            return ((nint)Value).ToInt64().ToString(format, formatProvider);
        }
        return ToString();
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return Equals((JetTableId)obj);
    }
    public override int GetHashCode()
    {
        return ((nint)Value).GetHashCode();
    }
    public bool Equals(JetTableId other)
    {
        return ((nint)Value).Equals(other.Value);
    }
    #endregion
}