namespace Discord_AI_Presence.Text_WebUI.Instructions
{
    public class ChatParameters
    {
        // Static because this should not change during runtime and will be consistent throughout instances of the object.
        private static readonly string[] parameterDefs = new string[]
        {
            " scenario: ",
            " preset: ",
            " firstmes: "
        };
        // Static because this should not change during runtime and will be consistent throughout instances of the object.
        private static readonly int paramDefLength = parameterDefs.Length;
        private readonly string originalMessage = string.Empty;
        private readonly Container[] parameters = new Container[paramDefLength];
        private int indexLocation = 0;

        /// <summary>
        /// Used to extract definitions to populate a custom scenario, preset or first message from a chat message
        /// rather than relying on slash commands or clunky bot commands, this can feel more natural.
        /// </summary>
        /// <param name="searchContent">The message sent in chat that is beginning a NEW AI chat session.</param>
        public ChatParameters(string characterName, string searchContent)
        {
            originalMessage = searchContent;
            var splitString = searchContent.Split(parameterDefs, StringSplitOptions.RemoveEmptyEntries);
            var positions = new int[paramDefLength];
            const int START_AT = 0;
            positions = FindIndexes(searchContent, START_AT, positions);
            parameters = PopulateContainers(searchContent, START_AT, positions, new Container[paramDefLength]);
            Array.Sort(parameters, (c1, c2) => c1.positionInList.CompareTo(c2.positionInList));
        }

        public string ApplyParamData(string charProfileData)
        {
            // scenario, preset, firstmes
            var paramData = GetParamData(parameterDefs[indexLocation]);
            indexLocation++;
            if(paramData == string.Empty)
            {
                return charProfileData;
            }
            return paramData;
        }

        /// <summary>
        /// Returns the specified parameter data if any custom parameters exists
        /// Can use scenario: or preset: or firstmes: or all in any order.
        /// This should be used when calling out an AI partner to chat example: "Hey (name) firstmes: "Hello!""
        /// This will replace the initial first message the AI would use with the one you've specified.
        /// </summary>
        /// <param name="definition">the parameter definition you will use</param>
        /// <returns>The parameter data extracted from the original message</returns>
        private string GetParamData(string definition)
        {
            // Returns -1 if not found.
            var containerLoc = GetContainerIndex(definition);
            if (containerLoc == -1)
                return string.Empty;
            var data = parameters[containerLoc];
            // If the parameter definition is the final entry on the list then we should only read to the end of the original message length
            var endPos = containerLoc == parameters.Length - 1 
                ? originalMessage.Length 
                : parameters[containerLoc + 1].positionInList;
            // Removes that parameter definition extracting only the data rather than doing a string.Replace.
            var startPos = data.positionInList + data.paramDef.Length - 1;
            string content = originalMessage.Substring(startPos, endPos - startPos);
            return content;
        }

        /// <summary>
        /// Populates the parameters list by extracting the relevant information from the message in chat.
        /// </summary>
        /// <param name="searchContent">The original message sent in chat that triggered this class.</param>
        /// <param name="indexLoc">The current location in the index.</param>
        /// <param name="positions">The locations of each parameter definition in the original message.</param>
        /// <param name="indexes">The array being constructed which will populate the parameters array.</param>
        /// <returns>A populated array containing the parameter definitions</returns>
        private static Container[] PopulateContainers(string searchContent, int indexLoc, int[] positions, Container[] indexes)
        {
            string def = parameterDefs[indexLoc];
            var nextData = new Container(positions[indexLoc], parameterDefs[indexLoc]);
            indexes[indexLoc] = nextData;
            indexLoc++;
            // Exit case
            if (indexLoc < paramDefLength)
                return PopulateContainers(searchContent, indexLoc, positions, indexes);
            return indexes;
        }

        /// <summary>
        /// Populates the locations within the original message of each parameter definition.
        /// This allows the definitions to be in any order and it will still extract the relevant information
        /// in their respective locations.
        /// </summary>
        /// <param name="searchContent">The original message sent in chat</param>
        /// <param name="indexLoc">The current index</param>
        /// <param name="indexes">An empty array of ints that will be populated with the definition locations</param>
        /// <returns>A populated int array with the locations of the definitions in the original message.</returns>
        private static int[] FindIndexes(string searchContent, int indexLoc, int[] indexes)
        {
            string def = parameterDefs[indexLoc];
            // Adding one to get rid of the preceeding white space instead of doing a trim
            int index = searchContent.IndexOf(def) + 1;
            indexes[indexLoc++] = index;
            // Exit case
            if (indexLoc < paramDefLength)
                return FindIndexes(searchContent, indexLoc, indexes);
            return indexes;
        }

        /// <summary>
        /// A simple method to get the index of where the definition specified is located.
        /// </summary>
        /// <param name="search">The parameter definition</param>
        /// <returns>Index >= 0 otherwise -1 if not found.</returns>
        private int GetContainerIndex(string search)
        {
            for (int i = 0; i < parameters.Length; i++)
                if (parameters[i].paramDef.Equals(search, StringComparison.OrdinalIgnoreCase))
                    return i;
            return -1;
        }

        private struct Container
        {
            public readonly string paramDef;
            public readonly int positionInList;

            public Container(int positionInList, string paramDef)
            {
                if (positionInList != 0)
                    this.paramDef = paramDef;
                else // If the position is 0 it means the parameter was not found in the original message
                    this.paramDef = string.Empty;
                this.positionInList = positionInList;
            }
        }
    }
}