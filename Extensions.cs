using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsSafePath(this string filename)
        {
            return WhitelistRegex.IsMatch(filename);
        }

        [GeneratedRegex("^[a-zA-Z0-9_-]+$")]
        private static partial Regex MyRegex();
    }
}
