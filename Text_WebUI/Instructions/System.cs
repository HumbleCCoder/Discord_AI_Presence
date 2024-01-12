using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.Instructions
{
    // WIP. Will be used for preset System messages. System messages help control unwanted AI behavior.
    public static class System
    {
        public static string IntroChatbot(string characterName)
        {
            return $"{characterName} is looking around the chat at everyone talking... hmmm... this is interesting";
        }
    }
}
