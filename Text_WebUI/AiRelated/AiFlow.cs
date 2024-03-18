using Discord.Commands;
using Discord_AI_Presence.Text_WebUI.DiscordStuff.API_Framework;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.AiRelated
{
    sealed internal class AiFlow
    {
        private readonly ProfileData _profile;
        private readonly string[] _wordTriggers;
        const int MaxValue = 500, MinValue = 0;
        private int _replyMax = MaxValue, _msgAmt;
        private readonly ulong _channelId;
        internal AiFlow(ProfileData profile, ulong channelId)
        {
            _profile = profile;
            _wordTriggers = new string[]
            {
                profile.NickOrName(),
            };
            _channelId = channelId;
        }

        /// <summary>
        /// Checks if the character's name of any trigger words are used first.
        /// Then checks if at least x many messages have been sent before doing a random roll to determine if they should reply.
        /// The ultimate goal is a contextual based reply system but it's a big challenge.
        /// </summary>
        /// <param name="msg">The message sent on the Discord server channel.</param>
        /// <returns></returns>
        internal async Task<bool> SendAiChat(string msg, SocketCommandContext scc)
        {
            if (_wordTriggers.Contains(msg))
                return true;
            if (_replyMax-- >= _msgAmt)
                return false;
            var randomNum = Extensions.ReturnRandom(MinValue, MaxValue);
            if (randomNum < _replyMax)
                return false;
            NewMsgAmt();
            var serverSettings = TextUI_Base.GetInstance().ServerData[scc.Guild.Id].ServerSettings;
            var webhook = TextUI_Base.GetInstance().Webhooks;
            await webhook.SendWebhookMessage(scc.Guild, _profile, serverSettings, msg, scc.Channel.Id);
            _replyMax = MaxValue;
            return true;
        }

        /// <summary>
        /// Prevents AI from responding randomly before a certain message threshold is met. 
        /// The more context the AI has to work with the less a chance of it sending a low quality reply or a repeat message.
        /// </summary>
        internal void NewMsgAmt()
        {
            _msgAmt = Extensions.ReturnRandom(4, 15);
        }
    }
}
