using Discord_AI_Presence.DebugThings;
using Discord_AI_Presence.Text_WebUI.DiscordStuff;
using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using Discord_AI_Presence.Text_WebUI.Presets;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.TextWebUI
{
    public class Connection
    {
        private const string TextWebGenerate = $"http://localhost:5000/v1/completions";

        /// <summary>
        /// Takes the MyData list which is a series of streamed JObjects and pieces them together to make it ready to post to Discord
        /// </summary>
        /// <param name="input">JObject string information</param>
        /// <param name="chatParticipants">Participants in the chat.</param>
        /// <param name="characterName">Name of the character</param>
        /// <returns>The AI message</returns>
        private static string RebuildString(List<MyData> input, List<string> chatParticipants, string characterName)
        {
            // Piecing together the streamable content
            var textValues = new List<string>();
            foreach (var item in input)
            {
                if (item != null && item.choices != null)
                    foreach (var c in item.choices)
                        textValues.Add(c.text);
            }
            var sb = new StringBuilder();
            /* 
             * Sometimes the AI will submit text with its own name in it like Bob: so we have to get rid of it otherwise the chat history would
             * look like this > Bob: Bob: "some text", this kind of clutter will degrade the quality of the chat and eventually the AI will
             * start writing garbage
             */
            chatParticipants.RemoveAll(x => x.Contains($"{characterName}:"));
            /* 
             * Same as above except the AI might try to speak for other people. While unavoidable at times, this
             * makes sure that the most blatent offenses are caught and never make it to the chat or memory.
             */
            foreach (var text in textValues)
            {
                bool append = true;
                foreach (var participants in chatParticipants)
                {
                    if (text.Contains($"{participants}:"))
                    {
                        append = false;
                        break;
                    }
                }
                if (append)
                    sb.Append(text);
            }
            // We don't need the \\r that some text files tend to generate. While it won't mess up the AI's replies, they do add to token count.
            return sb.Replace($"{characterName}:", string.Empty).Replace("\\r", string.Empty).ToString().Trim();
        }

        #region Classes necessarily to store json data from the neuro net.
        class Choice
        {
            public int index { get; init; }
            public object finish_reason { get; init; }
            public string text { get; init; }
            public Logprobs logprobs { get; init; }
        }

        class Logprobs
        {
            public List<object> top_logprobs { get; init; }
        }

        class MyData
        {
            public string id { get; init; }
            public string @object { get; init; }
            public int created { get; init; }
            public string model { get; init; }
            public List<Choice> choices { get; init; }
        }
        #endregion

        /// <summary>
        /// Submit a request to the neuro net and hopefully receive something valid back.
        /// </summary>
        /// <param name="characterProfile">The character profile</param>
        /// <param name="serverData">The server the chat is taking place in and its AI-related information</param>
        /// <param name="curChat">The current chat containing channel information and such</param>
        /// <param name="chatParticipants">The people who have talked in the chat while the AI is there.</param>
        /// <returns>Null if any errors are found or an empty string if the neuro net returns nothing.</returns>
        public static async Task<string> PostMessage(ProfileData characterProfile, TextUI_Servers serverData, Chats curChat, List<string> chatParticipants)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, TextWebGenerate);
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            curChat.Presets.CurPreset["prompt"] = curChat.PrintHistory(true);
            object replaceObject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(curChat.Presets.CurPreset));
            JsonSerializer.Create().Serialize(writer, replaceObject);
            writer.Flush();
            ms.Position = 0;
            
            request.Content = new StreamContent(ms);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
            {
                "Failed to connect to the server.".Dump();
                return null;
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var stream = responseContent.Split('\n').ToList();
            
            List<MyData> myDatas = [];
            foreach (var input in stream)
            {
                myDatas.Add(JsonConvert.DeserializeObject<MyData>(input.Replace("data: ", string.Empty).Replace("Data: ", string.Empty)));
            }
            return RebuildString(myDatas, chatParticipants, characterProfile.NickOrName());
        }

    }
}
