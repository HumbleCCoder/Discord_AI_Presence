using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord_AI_Presence.DebugThings;
using Discord_AI_Presence.Text_WebUI.Instructions;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.Button_Related
{
    /// <rummary>
    /// These are functions to be used with button clicks relating to the text AI
    /// </rummary>
    internal class ButtonHandling
    {
        public const string NextID = "nextChar";
        public const string OKID = "OK";
        private readonly ButtonBuilder _nextChar, _selectChar;
        private readonly RestUserMessage _rum;
        private readonly List<ProfileData> _profiles;
        private readonly ulong _startChanId;
        private int _indexer, _current;
        private Scenario.ScenarioPresets _chatType;

        public ButtonHandling(List<ProfileData> profiles, RestUserMessage rum, ulong startChanId, Scenario.ScenarioPresets chatType)
        {
            _nextChar = MakeButton("nextChar", "Next");
            _selectChar = MakeButton("OK", "Select");
            _profiles = profiles;
            _rum = rum;
            _startChanId = startChanId;
            _chatType = chatType;
        }
        private int Index
        {
            get { return _indexer; }
            set
            {
                _indexer = value;
                if (_indexer >= _profiles.Count)
                    _indexer = 0;
                if(_indexer < 0)
                    _indexer = 0;
            }
        }

        private static ButtonBuilder MakeButton(string id, string label)
        {
            ButtonBuilder bb = new()
            {
                CustomId = id,
                Style = ButtonStyle.Primary,
                Label = label
            };
            return bb;
        }


        /// <rummary>
        /// Edits the message
        /// </rummary>
        /// <param name="rum">The Rest User Message</param>
        /// <returns></returns>
        public async Task SwapMessage()
        {
            _current = Index;
            await _rum.ModifyAsync(x => x.Content = $"There are multiple characters. Please pick one : {_profiles[Index].NickOrName()}\n{_profiles[Index].AvatarUrl}");
            var builder = new ComponentBuilder();
            builder.WithButton(_nextChar, 0);
            builder.WithButton(_selectChar, 0);
            await _rum.ModifyAsync(x => x.Components = builder.Build());
            Index++;
        }

        public async Task ChooseCharacter(SocketMessageComponent smc)
        {
            string defaultPreset = Presets.TextUI_Presets.DefaultPreset;
            await TextUI_Base.GetInstance().StartChat(smc, _profiles[_current], _current, _chatType);
            await _rum.DeleteAsync();
            TextUI_Base.GetInstance().ServerData[(ulong)smc.GuildId].DuplicateHandling.Remove(_startChanId);
        }
    }
}
