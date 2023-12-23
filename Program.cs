using Discord_AI_Presence.Text_WebUI;
using Discord_AI_Presence.Text_WebUI.Presets;

namespace Discord_AI_Presence
{
    internal class Program
    {
        static void Main(string[] args)
        {
            foreach (var g in TextUI_Base.GetInstance().Cards)
            {
                Console.WriteLine(g.Key);
                Console.WriteLine(g.Value);
            }
        }
    }
}