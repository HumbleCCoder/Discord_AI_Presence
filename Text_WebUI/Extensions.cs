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
        /// Removes all URLs and custom server emojis from the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string CleanMessage(this string message)
        {
            message = message.CleanURL();
            message = message.RemoveEmojis();
            return message;
        }

        /// <summary>
        /// Checks if it contains a URL or Emoji. Might not be necessary
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ContainsURLOrEmoji(string message)
        {
            return HasEmoji(message) || HasURL(message);
        }

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

        /* Having URLs or custom server emojis seems to bug the AI making them spit out garbage forever until the message is erased or the chat is restarted
         As a result we have to make sure none of it enters the neuro net.
         */
        #region Regex Checks

        private static string CleanURL(this string message)
        {
            string pattern = @"(http|https)[^\s/$.?#].[^\s]*";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return Regex.Replace(message, pattern, string.Empty).Replace("  ", " "); ;
        }

        private static string RemoveEmojis(this string message)
        {
            if (string.IsNullOrEmpty(message))
                return string.Empty;
            string pattern = @"<@\d+>|<@!\d+>|:\w+:|<:[^:]+:\d+>";
            return Regex.Replace(message, pattern, string.Empty).Replace("  ", " ");
        }

        private static bool HasEmoji(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;
            string pattern = @"<@\d+>|<@!\d+>|:\w+:|<:[^:]+:\d+>";
            return Regex.IsMatch(message, pattern);
        }

        private static bool HasURL(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;
            string pattern = @"(http|https)[^\s/$.?#].[^\s]*";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return Regex.IsMatch(message, pattern);
        }
        #endregion

    }
}
