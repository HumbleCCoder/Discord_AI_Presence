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

        private static string RebuildString(List<MyData> input, List<string> chatParticipants, string characterName)
        {
            var textValues = new List<string>();
            foreach (var item in input)
            {
                if (item != null && item.choices != null)
                    foreach (var c in item.choices)
                        textValues.Add(c.text);
            }
            var sb = new StringBuilder();
            chatParticipants.RemoveAll(x => x.Contains($"{characterName}:"));
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
            return sb.Replace($"{characterName}:", string.Empty).Replace("\\n", string.Empty).Replace("\\r", string.Empty).ToString().Trim();
        }
        public class Choice
        {
            public int index { get; set; }
            public object finish_reason { get; set; }
            public string text { get; set; }
            public Logprobs logprobs { get; set; }
        }

        public class Logprobs
        {
            public List<object> top_logprobs { get; set; }
        }

        public class MyData
        {
            public string id { get; set; }
            public string @object { get; set; }
            public int created { get; set; }
            public string model { get; set; }
            public List<Choice> choices { get; set; }
        }

        public static async Task<string> PostMessage(ProfileData characterProfile, TextUI_Servers serverData, Chats curChat, List<string> chatParticipants)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, TextWebGenerate);
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);
            curChat.Presets.CurPreset["prompt"] = curChat.PrintHistory(true);
            object replaceObject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(curChat.Presets.CurPreset));
            Console.Write(replaceObject);
            JsonSerializer.Create().Serialize(writer, replaceObject);
            writer.Flush();
            ms.Position = 0;
            
            request.Content = new StreamContent(ms);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            Console.Write("Request Content: " + request.Content);
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to connect to the server.");
                return null;
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var stream = responseContent.Split('\n').ToList();
            
            Console.WriteLine(responseContent);
            List<MyData> myDatas = [];
            foreach (var input in stream)
            {
                myDatas.Add(JsonConvert.DeserializeObject<MyData>(input.Replace("data: ", string.Empty).Replace("Data: ", string.Empty)));
            }
            return RebuildString(myDatas, chatParticipants, characterProfile.NickOrName());
        }

    }
}
