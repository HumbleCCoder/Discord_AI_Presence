using Discord.Commands;
using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;

namespace Discord_AI_Presence.Text_WebUI.TextWebUI
{
    public interface ITextUIBase
    {        
        public abstract Task StartChat(SocketCommandContext scc, string characterName);
        public abstract Task StartChat(SocketCommandContext scc, string characterName, string customScenario = "");
        public abstract void PopulateServerData(List<SocketGuild> guilds);
        public abstract static TextUI_Base GetInstance();
        public abstract static ProfileData MatchName(string charFirstName);
        public abstract string ProfileLocation(string filename = "");
        public abstract void PopulateProfileDictionary();
        public abstract ProfileData Check(string msg);
    }
}
