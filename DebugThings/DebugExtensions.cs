using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.DebugThings
{
    public static class DebugExtensions
    {
        /// <summary>
        /// Print out every detail of an exception.
        /// </summary>
        /// <param name="m">The Exception</param>
        public static void Log(this Exception m)
        {
            string text = $"{m.Message}, {m.TargetSite}, {m.Source}, {m.InnerException}, {m.StackTrace}, {m.HResult}, {m.Data}, {m.HelpLink}";
            Console.WriteLine(text);
        }

        /// <summary>
        /// Lazy person's console print.
        /// </summary>
        /// <param name="value">the object to be printed to console</param>
        public static void Dump(this object value)
        {
            Console.WriteLine(value.ToString());
        }
    }
}