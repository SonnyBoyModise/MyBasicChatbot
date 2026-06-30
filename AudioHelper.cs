// AudioHelper.cs — plays the WAV voice greeting on startup

using System.Media;

namespace MyBasicChatbot.Helpers
{
    public static class AudioHelper
    {
        public static void PlayGreeting(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using SoundPlayer player = new SoundPlayer(filePath);
                    player.Play(); // async so UI loads while audio plays
                }
            }
            catch { /* audio errors are non-fatal */ }
        }
    }
}
