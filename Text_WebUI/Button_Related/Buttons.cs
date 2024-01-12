using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;

namespace Discord_AI_Presence.Text_WebUI.Button_Related
{
    public readonly struct Buttons
    {
        private readonly ButtonComponent _button;

        public Buttons(string customId, string label)
        {
            ButtonBuilder bb = new()
            {
                CustomId = customId,
                Style = ButtonStyle.Primary,
                Label = label
            };
            _button = bb.Build();
        }
    }
}
