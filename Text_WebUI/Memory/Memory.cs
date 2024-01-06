namespace Discord_AI_Presence.Text_WebUI.MemoryManagement
{
    /// <summary>
    /// The struct is mostly readonly. A struct is used because of how frequently messages will be created when one or many chat sessions are going on
    /// </summary>
    public struct Memory
    {
        /// <summary>
        /// It can't be readonly due to how message editing is common with AI chats in order to correct AI mistakes or push AI towards specific directions.
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Username. This will either be the person's Discord username (or display name/Server nickname) or the AI's character name.
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// Discord static user ID. Will be useful in case a more specific search is required.
        /// </summary>
        public ulong UserID { get; init; }
        /// <summary>
        /// Discord message ID for finding and identifying the exact message on Discord
        /// </summary>
        public ulong MsgID { get; init; }
        public Memory(string Message, string Name, ulong UserID, ulong MsgID) =>
            (this.Message, this.Name, this.UserID, this.MsgID) = (Message, Name, UserID, MsgID);

        /// <summary>
        /// Edits the message. Edit messaging is pretty standard practice due to how AI can frequently send unwanted messages in an otherwise good reply.
        /// Does not edit the message on Discord.
        /// </summary>
        /// <param name="message">The message to edit.</param>
        public void EditMessage(string message) => Message = message;

        /// <summary>
        /// Returns Name: Message text format.
        /// </summary>
        /// <returns></returns>
        public readonly string Format() => $"{Name}: {Message}";

        /// <summary>
        /// Finds the topmost message that matches for use in a Linq operator.
        /// </summary>
        /// <param name="message">The message to compare. The comparison is not case sensitive.</param>
        /// <returns>Returns true if found.</returns>
        public readonly bool IsMessage(string message) => Message.Equals(message, StringComparison.OrdinalIgnoreCase);
    }
}
