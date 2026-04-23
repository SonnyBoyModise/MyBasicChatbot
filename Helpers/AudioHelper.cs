// ============================================================
// AudioHelper.cs
// Responsible for playing the WAV voice greeting when the
// application launches. Uses System.Media.SoundPlayer which
// is available on Windows (net8.0-windows target).
// ============================================================

using System.Media;

namespace MyBasicChatbot.Helpers;

public static class AudioHelper
{
    /// <summary>
    /// Plays the WAV file at the given path synchronously (waits for it
    /// to finish before the application continues). If the file is missing
    /// or an error occurs, a short notice is printed and the app continues.
    /// </summary>
    /// <param name="filePath">Relative or absolute path to the .wav file.</param>
    public static void PlayGreeting(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                // PlaySync() blocks until the audio finishes — ensures the
                // greeting completes before the logo is shown
                using SoundPlayer player = new SoundPlayer(filePath);
                player.PlaySync();
            }
            else
            {
                // File missing: warn the user but don't crash the app
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  [Voice greeting file not found — skipping audio]");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            // Any audio error (driver issue, corrupt file, etc.) is non-fatal
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  [Audio playback error: {ex.Message}]");
            Console.ResetColor();
        }
    }
}
