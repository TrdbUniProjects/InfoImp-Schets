using System.Numerics;
using System.Text.RegularExpressions;

namespace Schets.Util; 

public static class NumberUtils {
    private static readonly Regex IntRegex = new Regex("^\\d*$");
    
    public static bool IsStringValidInt(string value) {
        if (!IntRegex.IsMatch(value)) return false;

        BigInteger bi = BigInteger.Parse(value);
        return bi <= int.MaxValue;
    }
    
}