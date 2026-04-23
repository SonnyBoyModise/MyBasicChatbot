// ============================================================
// DisplayHelper.cs
// Handles all visual output for the chatbot:
//   - ASCII art logo
//   - Welcome banner
//   - Coloured text helpers
//   - Typing (typewriter) effect for a conversational feel
//   - Section headers and dividers for structure
// ============================================================

namespace MyBasicChatbot.Helpers;

public static class DisplayHelper
{
    // ----------------------------------------------------------------
    // ASCII Logo
    // ----------------------------------------------------------------

    /// <summary>
    /// Prints a large ASCII art logo and subtitle to the console.
    /// Displayed once at application startup.
    /// </summary>
    public static void ShowLogo()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine(@"   ██████╗██╗   ██╗██████╗ ███████╗██████╗ ██████╗  ██████╗ ████████╗");
        Console.WriteLine(@"  ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗██╔══██╗██╔═══██╗╚══██╔══╝");
        Console.WriteLine(@"  ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝██████╔╝██║   ██║   ██║   ");
        Console.WriteLine(@"  ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗██╔══██╗██║   ██║   ██║   ");
        Console.WriteLine(@"  ╚██████╗   ██║   ██████╔╝███████╗██║  ██║██████╔╝╚██████╔╝   ██║   ");
        Console.WriteLine(@"   ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝╚═════╝  ╚═════╝   ╚═╝   ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("        🔐  CYBERSECURITY AWARENESS BOT  ·  South Africa  🔐");
        Console.WriteLine("              Powered by the Department of Cybersecurity");
        Console.ResetColor();
        Console.WriteLine();
    }

    // ----------------------------------------------------------------
    // Welcome Banner
    // ----------------------------------------------------------------

    /// <summary>
    /// Prints a bordered welcome banner showing the main topics the
    /// chatbot can help with.
    /// </summary>
    public static void ShowWelcomeBanner()
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║   Helping South African citizens stay safe online.           ║");
        Console.WriteLine("  ║   Topics: Phishing | Passwords | Safe Browsing | Malware     ║");
        Console.WriteLine("  ║           Social Engineering | SA Cyber Law                  ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    // ----------------------------------------------------------------
    // Section Headers & Dividers
    // ----------------------------------------------------------------

    /// <summary>
    /// Prints a styled section header in magenta.
    /// </summary>
    /// <param name="title">The section title to display.</param>
    public static void ShowSectionHeader(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  ┌─ {title.ToUpper()} " + new string('─', Math.Max(0, 50 - title.Length)));
        Console.ResetColor();
    }

    /// <summary>
    /// Prints a horizontal divider line to visually separate exchanges.
    /// </summary>
    public static void ShowDivider()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  ──────────────────────────────────────────────────────────────");
        Console.ResetColor();
    }

    // ----------------------------------------------------------------
    // Typing Effect
    // ----------------------------------------------------------------

    /// <summary>
    /// Prints text one character at a time with a short delay, simulating
    /// a typewriter / conversational typing effect.
    /// </summary>
    /// <param name="text">The string to print.</param>
    /// <param name="colour">Console colour to use (default White).</param>
    /// <param name="delayMs">Milliseconds between each character (default 22ms).</param>
    public static void TypeText(string text, ConsoleColor colour = ConsoleColor.White, int delayMs = 22)
    {
        Console.ForegroundColor = colour;
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delayMs);
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    // ----------------------------------------------------------------
    // Bot / User Output Helpers
    // ----------------------------------------------------------------

    /// <summary>
    /// Displays a bot message with a yellow "🤖 Bot:" prefix and
    /// typing effect.
    /// </summary>
    /// <param name="message">The message the bot wants to say.</param>
    public static void BotSay(string message)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("  🤖 Bot: ");
        Console.ResetColor();
        TypeText(message, ConsoleColor.White, 20);
    }

    /// <summary>
    /// Prints the green "👤 You:" prompt and waits for user input.
    /// </summary>
    public static void UserPrompt()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("  👤 You: ");
        Console.ResetColor();
    }

    /// <summary>
    /// Displays an error/warning message in red with a warning icon.
    /// Used for input validation feedback.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    public static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  ⚠️   {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Displays an informational tip in dark yellow.
    /// </summary>
    /// <param name="tip">The tip text to display.</param>
    public static void ShowTip(string tip)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"\n  💡 Tip: {tip}");
        Console.ResetColor();
    }
}
