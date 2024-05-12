using Discord.Commands;
using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;

namespace Discord_AI_Presence.Text_WebUI.TextWebUI
{
    public interface ITextUIBase
    {        
        public abstract Task StartChat(SocketCommandContext scc, string characterName, int index = 0);
        public abstract void PopulateServerData(List<SocketGuild> guilds);
        public abstract static TextUI_Base GetInstance();
        public abstract static List<ProfileData> MatchName(string charFirstName);
        public abstract string ProfileLocation(string filename = "");
        public abstract void PopulateProfileDictionary();
        public abstract List<ProfileData> Check(string msg, bool existsInChat);
    }
}
