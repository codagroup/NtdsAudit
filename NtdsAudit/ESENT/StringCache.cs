using Microsoft.Isam.Esent.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace CODA.NtdsAudit.ESENT;
/// <summary>
/// A mock up representation of <see href="https://learn.microsoft.com/en-us/windows/win32/extensible-storage-engine/microsoft.isam.esent.interop-namespace"/> StringCache internal class without the esent.dll dependency.
/// </summary>
internal static class StringCache
{
    #region Properties
    private const int MaxLengthToCache = 128;

    private const int NumCachedBoxedValues = 1031;

    private readonly static Dictionary<uint,string> _cache = [];
    private readonly static Encoding _encoding = Encoding.Unicode;
    #endregion
    #region Functions
    public static string TryToIntern(string s)
    {
        if (s is null || s == string.Empty)
        {
            return string.Empty;
        }
        else
        {
            return string.IsInterned(s) ?? s;
        }
    }

    public static string GetString(byte[] value, int startIndex, int count)
    {
        if (count == 0 || value.Length < (startIndex + count))
        {
            return string.Empty;
        }
        else
        {
            uint hash = CalculateHash(value.AsSpan(startIndex, count));
            string? cachedString = null;

            if (_cache.ContainsKey(hash))
            {
                cachedString = _cache[hash];
            }
            else
            {
                _cache[hash] = _encoding.GetString(value, startIndex, count);
                cachedString = _cache[hash];
            }
            return cachedString ?? string.Empty;
        }
    }

    private static uint CalculateHash(ReadOnlySpan<byte> value)
    {
        uint hash = 17; //because who starts at 0.....
        foreach (byte b in value)
        {
            hash *= 31;
            hash += b;
        }
        return hash;
    }
    #endregion
}