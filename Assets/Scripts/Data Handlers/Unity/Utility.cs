using System;

namespace TheWarriors
{
    public static class Utility
    {
        public static UInt32 ConvertToHex(string hexValue)
        {
            if (hexValue.Length != 8)
            {
                throw new ArgumentException("*** Error: Input string must be 8 characters long.");
            }

            if (UInt32.TryParse(hexValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out uint result))
            {
                return result;
            }

            throw new ArgumentException("*** Error: Failed to convert the input string to UInt32.");
        }
    }
}
