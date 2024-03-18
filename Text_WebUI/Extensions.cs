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
        public static int ReturnRandom(int minValue, int maxValue)
        {
            return RandomNumberGenerator.GetInt32(minValue, maxValue + 1);
        }

        private static Random _rand = new();

        public static IList<T> Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static bool VeryRare(double rarityThreshold)
        {
            Random random = new Random();

            double randomValue = random.NextDouble();
            return randomValue < rarityThreshold;
        }
    }
}
