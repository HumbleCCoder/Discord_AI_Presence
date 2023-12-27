namespace Discord_AI_Presence.Text_WebUI.MemoryManagement
{
    public struct Memory
    {
        public string Message { get; private set; }
        /// <summary>
        /// Username. This will either be the person's Discord username (or display name/Server nickname) or the AI's character name.
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Discord static user ID. Will be useful in case a more specific search is required.
        /// </summary>
        public ulong UserID { get; init; }
        public Memory(string message, string name, ulong userID) =>
            (Message, Name, UserID) = (message, name, userID);

        /// <summary>
        /// Edits the message. Edit messaging is pretty standard practice due to how AI can frequently send unwanted messages in an otherwise good reply.
        /// </summary>
        /// <param name="message">The message to edit.</param>
        public void EditMessage(string message) => Message = message;

        /// <summary>
        /// Finds the topmost message that matches for use in a Linq operator.
        /// </summary>
        /// <param name="message">The message to compare. The comparison is not case sensitive.</param>
        /// <returns>Returns true if found.</returns>
        public readonly bool IsMessage(string message) => Message.Equals(message, StringComparison.OrdinalIgnoreCase);
    }
}
