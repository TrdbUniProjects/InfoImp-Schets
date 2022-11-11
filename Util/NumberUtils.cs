using System.Numerics;
using System.Text.RegularExpressions;

namespace Schets.Util; 

public static class NumberUtils {
    /// <summary>
    /// Regex for checking whether a string is a valid int
    /// </summary>
    private static readonly Regex IntRegex = new Regex("^\\d*$");
    
    /// <summary>
    /// Check if a string is a valid string
    /// </summary>
    /// <param name="value">Non-empty string to check</param>
    /// <returns>True if the string is a valid int</returns>
    public static bool IsStringValidInt(string value) {
        if (!IntRegex.IsMatch(value)) return false;

        BigInteger bi = BigInteger.Parse(value);
        return bi <= int.MaxValue;
    }
    
}