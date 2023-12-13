using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Stable_Diffusion
{
    public class SD_Base
    {
        private static SD_Base _instance = null!;
        private static readonly object _lock = new();

        private SD_Base()
        {

        }

        public static SD_Base GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new SD_Base();
                }
            }
            return _instance;
        }

        /// <summary>
        /// Don't forget to include the extension in the filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string Configuration(string fileName)
        {
            return $@"{Environment.CurrentDirectory}\Configuration\{fileName}";
        }
    }
}
