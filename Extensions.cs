using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord_AI_Presence
{
    public static partial class Extensions
    {
        private static readonly Regex WhitelistRegex = MyRegex();

        /// <summary>
        /// White list to make sure a filename submitted is not attempting a path traversal attack.
        /// </summary>
        /// <param name="filename">the filename</param>
        /// <returns></returns>
        public static bool IsSafePath(this string filename)
        {
            return WhitelistRegex.IsMatch(filename);
        }

        [GeneratedRegex("^[a-zA-Z0-9_-]+$")]
        private static partial Regex MyRegex();
        public static int ReturnRandom(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];
            maximumValue--;
            RandomNumberGenerator.Create().GetBytes(randomNumber);
            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);
            int range = maximumValue - minimumValue + 1;
            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }
    }
}
