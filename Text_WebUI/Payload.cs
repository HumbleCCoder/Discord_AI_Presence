using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord_AI_Presence.Text_WebUI.Presets;

namespace Discord_AI_Presence.Text_WebUI
{
    internal class Payload
    {
        /// <summary>
        /// Submits an object as an anonymous object initializer.
        /// </summary>
        /// <param name="memoryDump"></param>
        /// <param name="chatParticipants"></param>
        /// <param name="presets"></param>
        /// <returns></returns>
        public object PayloadMaker(string memoryDump, string[] chatParticipants, TextUI_Presets presets)
        {
            var thisSettings = new
            {
                prompt = memoryDump,//string
                max_new_tokens = 200,
                do_sample = true,
                temperature = 1,
                top_p = 1,
                typical_p = 1,
                repetition_penalty = 1,
                encoder_repetition_penalty = 1,
                top_k = 0,
                min_length = 0,
                no_repeat_ngram_size = 0,
                num_beams = 1,
                penalty_alpha = 0,
                length_penalty = 1,
                early_stopping = false,
                seed = -1,
                add_bos_token = true,
                stopping_strings = chatParticipants,//string array
                truncation_length = 4096,//int
                ban_eos_token = false,
                skip_special_tokens = true,
                top_a = 0,
                tfs = 1,
                epsilon_cutoff = 0,
                eta_cutoff = 0,
                mirostat_mode = 2,
                mirostat_tau = 4,
                mirostat_eta = 0.1f
            };
            return thisSettings;
        }
    }
}
