using Discord.WebSocket;
using System.Collections.Specialized;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using System.Net.Http.Headers;
using System.Text;
using Discord_AI_Presence.DebugThings;

namespace Discord_AI_Presence.Text_WebUI.DiscordStuff.API_Framework
{
    public class Webhooks
    {
        public async Task SendWebhookMessage(SocketGuild sg, ProfileData characterProfile, Settings serverSettings, string neuroMsg, ulong channelID)
        {
            try
            {
                var webhooks = await sg.GetWebhooksAsync();
                if (webhooks == null || webhooks.Count == 0) 
                    return;
                var url = !string.IsNullOrEmpty(serverSettings.Webhooks) ? serverSettings.Webhooks :
                    $"https://discord.com/api/webhooks/{webhooks.First().Id}/{webhooks.First().Token}";
                await webhooks.First().ModifyAsync(x => x.ChannelId = channelID);
                var webhookPayload = new
                {
                    username = characterProfile.NickOrName(),
                    avatar_url = characterProfile.AvatarUrl,
                    content = neuroMsg,
                };
                await PostToWebhook(url, webhookPayload);
            }
            catch (Exception m)
            {
                DebugThings.DebugExtensions.Log(m);
            }
        }
        private static async Task PostToWebhook(string webhookUrl, object data)
        {
            using var httpClient = new HttpClient();
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(webhookUrl, content);
        }
    }
}
