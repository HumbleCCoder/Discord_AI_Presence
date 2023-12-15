namespace Discord_AI_Presence.Text_WebUI.Presets
{
    public class PresetData
    {
        #region properties
        public int max_new_tokens { get; init; }
        public int truncation_length { get; init; }
        public bool add_bos_token { get; init; }
        public bool ban_eos_token { get; init; }
        public bool skip_special_tokens { get; init; }
        public float temperature { get; init; }
        public float top_p { get; init; }
        public int top_k { get; init; }
        public float typical_p { get; init; }
        public float top_a { get; init; }
        public float tfs { get; init; }
        public float epsilon_cutoff { get; init; }
        public float eta_cutoff { get; init; }
        public float repetition_penalty { get; init; }
        public int no_repeat_ngram_size { get; init; }
        public float penalty_alpha { get; init; }
        public int num_beams { get; init; }
        public float length_penalty { get; init; }
        public int min_length { get; init; }
        public float encoder_repetition_penalty { get; init; }
        public bool do_sample { get; init; }
        public bool early_stopping { get; init; }
        public int mirostat_mode { get; init; }
        public int mirostat_tau { get; init; }
        public float mirostat_eta { get; init; }
        #endregion

        /// <summary>
        /// Memory is automatically truncated if over the limit. Chat Participants are everyone in the chat who has talked in the same channel that an AI is engaged in.
        /// </summary>
        /// <param name="memoryDump"></param>
        /// <param name="chatParticipants"></param>
        /// <returns></returns>
        public object Payload(string memoryDump, string[] chatParticipants)
        {
            var settings = new
            {
                prompt = memoryDump,//string
                max_new_tokens,//int
                do_sample,
                temperature,
                top_p,
                typical_p,
                repetition_penalty,
                encoder_repetition_penalty,
                top_k,
                min_length,
                no_repeat_ngram_size,
                num_beams,
                penalty_alpha,
                length_penalty,
                early_stopping,
                seed = -1,
                add_bos_token,
                stopping_strings = chatParticipants,//string array
                truncation_length,//int
                ban_eos_token,
                skip_special_tokens,
                top_a,
                tfs,
                epsilon_cutoff,
                eta_cutoff,
                mirostat_mode,
                mirostat_tau,
                mirostat_eta
            };
            return settings;
        }

        public PresetData() { }
    }
}
