using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.TextWebUI
{
    // WIP. The AI may have many neuro net queues at the same time and it could possible lead to the neuro net getting overwhelmed.
    public class Queues
    {
        public delegate Task MyDelegate(string memory);
    }
}
