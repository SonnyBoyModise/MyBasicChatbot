// ============================================================
// MyBasicChatbot - Cybersecurity Awareness Assistant
// Entry point: sets up the console, plays the voice greeting,
// displays the logo, then hands control to the ChatBot class.
// ============================================================

using MyBasicChatbot.Helpers;

class Program
{
    static void Main(string[] args)
    {
        // Set the console window title
        Console.Title = "Cybersecurity Awareness Bot - South Africa";

        // Make the console window large enough to look good
        try
        {
            Console.WindowWidth  = Math.Max(Console.WindowWidth,  90);
            Console.WindowHeight = Math.Max(Console.WindowHeight, 40);
        }
        catch
        {
            // Some terminals don't support resizing — silently ignore
        }

        // Step 1: Play the recorded WAV voice greeting
        // The file lives in Audio/welcome.wav (copied to output dir on build)
        AudioHelper.PlayGreeting("Audio/welcome.wav");

        // Step 2: Display ASCII logo and welcome banner
        DisplayHelper.ShowLogo();
        DisplayHelper.ShowWelcomeBanner();

        // Step 3: Start the interactive chatbot session
        ChatBot bot = new ChatBot();
        bot.Start();
    }
}
