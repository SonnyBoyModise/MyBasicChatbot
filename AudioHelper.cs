// ============================================================
// Helpers/AudioHelper.cs
// Plays the WAV voice greeting when the application launches.
// Uses System.Media.SoundPlayer (available on net8.0-windows).
// Identical to Part 1 but now called from WPF startup.
// ============================================================

using System.Media;

namespace MyBasicChatbot.Helpers
{
    public static class AudioHelper
    {
        /// <summary>
        /// Plays the WAV file at the given path asynchronously so the
        /// WPF window can load while audio plays in the background.
        /// Falls back gracefully if the file is missing or audio fails.
        /// </summary>
        public static void PlayGreeting(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // Play asynchronously so UI isn't blocked during startup
                    using SoundPlayer player = new SoundPlayer(filePath);
                    player.Play();
                }
                // Silently skip if file not found — don't crash the app
            }
            catch (Exception)
            {
                // Any audio error is non-fatal — silently ignore
            }
        }
    }
}
