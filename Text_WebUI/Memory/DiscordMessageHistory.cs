using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Discord_AI_Presence.Text_WebUI.MemoryManagement
{
    /// <summary>
    /// In channels dedicated to AI chat, we need to grab the history.
    /// When it detects the end chat message, it will populate the message history for everything after it
    /// This allows chat history to remain stored on Discord instead of locally and repopulate automatically.
    /// </summary>
    public static class DiscordMessageHistory
    {
        private const string NEW_CHAT_PHRASE = "ENDCHAT";

        /// <summary>
        /// Populates a new list of chat history based on the last 1000 messages sent in chat.
        /// It looks for the phrase above in order to know when a new chat has begun.
        /// All chat prior to that phrase is not added.
        /// </summary>
        /// <param name="stc">The Discord server channel to search in.</param>
        /// <returns></returns>
        public static async Task<List<Memory>> PopulateHistory(SocketTextChannel stc)
        {
            var cacheMsgs = (await stc.GetMessagesAsync(1000).FlattenAsync()).Reverse().ToList();
            var startAt = cacheMsgs.FindLastIndex(x => x.Content.Equals(NEW_CHAT_PHRASE));
            List<Memory> mem = [];
            for (int i = startAt + 1; i < cacheMsgs.Count; i++)
            {
                var msg = cacheMsgs[i];
                mem.Add(new Memory(msg.Content,msg.Author.GlobalName ?? msg.Author.Username,msg.Author.Id,msg.Id));
            }
            return mem;
        }

        /// <summary>
        /// Checks whether the chat should be ended or not.
        /// I decided to add this here since the phrase would be the same as the constant used. If the API user wants to change it they can.
        /// </summary>
        /// <param name="sum">The message sent by the user in the Discord server.</param>
        /// <returns>Returns false if the phrase is not found. Note: it is case sensitive and must be all caps</returns>
        public static bool ShouldEndChat(SocketUserMessage sum)
        {
            return sum.Content.Equals(NEW_CHAT_PHRASE) && !sum.Author.IsBot && !sum.Author.IsWebhook;
        }
    }
}
