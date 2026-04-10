#region Usings
using System.Runtime.CompilerServices;
#endregion

namespace CODA.NtdsAudit.ESENT;

/// <summary>
/// A standalone mock up of <see href="https://learn.microsoft.com/en-gb/windows/win32/extensible-storage-engine/columnvalue-class"/> without the ESENT dependency
/// </summary>
public abstract class ColumnValue
{
    #region Fields
    private object? _value;
    #endregion
    #region Properties
    /// <summary>
    /// Gets or sets the column identifier
    /// </summary>
    public uint ColumnId { get; set; }
    /// <summary>
    /// Indicates potential column problems when parsing data values
    /// </summary>
    public JetWarningType Warning { get; internal set; }
    /// <summary>
    /// 
    /// </summary>
    public abstract int Length { get; }
    /// <summary>
    /// 
    /// </summary>
    protected abstract int Size { get; }

    public virtual object? Value
    {
        get
        {
            return _value; 
        }
        set
        {
            _value = value!;
            Warning = value is null ? JetWarningType.ColumnNull : JetWarningType.Success;
        }
    }

    #endregion
    #region Functions
    protected abstract void GetValueFromBytes(byte[] value, int startIndex, int count);
    #endregion
    #region Overrides
    /// <summary>
    /// Returns a string representation of the Value, or an empty string if Value is null. Should be overridden in inheriting classes.
    /// </summary>
    /// <returns>string representation of <see cref="ColumnValue.Value"/></returns>
    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
    #endregion
}
/// <summary>
/// A standalone mock up of <see href="https://learn.microsoft.com/en-gb/windows/win32/extensible-storage-engine/columnvalueofstruct-t-class"/> without the ESENT dependency
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ColumnValueOfType<T> : ColumnValue where T : struct, IEquatable<T>
{
    #region Fields
    private T? _value;
    #endregion
    #region Properties
    new public T? Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            base.Warning = value is null ? JetWarningType.ColumnNull : JetWarningType.Success;
        }
    }
    public override int Length
    {
        get
        {
            if (!Value.HasValue)
            {
                return 0;
            }

            return Size;
        }
    }
    #endregion
    #region Overrides
    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
    #endregion
    #region Functions
    protected void CheckDataCount(int count)
    {
        if (Size != count)
        {
            throw new InvalidOperationException();
        }
    }
    #endregion
}
