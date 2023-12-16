using Discord_AI_Presence.Stable_Diffusion;
using Discord_AI_Presence.Text_WebUI.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI
{
    public class TextUI_Base
    {
        private static TextUI_Base _instance = null!;
        private static readonly object _lock = new();
        public TextUI_Presets Presets { get; init; }

        public TextUI_Base()
        {
            Presets = new TextUI_Presets();
        }

        public static TextUI_Base GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new TextUI_Base();
                }
            }
            return _instance;
        }
    }
}
