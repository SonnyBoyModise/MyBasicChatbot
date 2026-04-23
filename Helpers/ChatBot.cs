// ============================================================
// ChatBot.cs
// The core chatbot class. Manages the full user session:
//   1. Greets the user and captures their name
//   2. Runs the main conversation loop
//   3. Delegates display to DisplayHelper
//   4. Delegates response lookup to ResponseEngine
//   5. Validates all user input before processing
// ============================================================

namespace MyBasicChatbot.Helpers;

public class ChatBot
{
    // ----------------------------------------------------------------
    // Auto-Properties
    // ----------------------------------------------------------------

    /// <summary>
    /// Stores the user's name after they enter it during the greeting.
    /// Used to personalise all subsequent bot messages.
    /// Auto-property with a private setter so only this class can set it.
    /// </summary>
    public string UserName { get; private set; } = string.Empty;

    /// <summary>
    /// Tracks the total number of messages exchanged in the session.
    /// Displayed in the farewell message for a personal touch.
    /// </summary>
    public int MessageCount { get; private set; } = 0;

    // ----------------------------------------------------------------
    // Public Entry Point
    // ----------------------------------------------------------------

    /// <summary>
    /// Starts the chatbot session. Calls GreetUser() first to capture
    /// the user's name, then enters the main conversation loop.
    /// </summary>
    public void Start()
    {
        GreetUser();
        RunConversationLoop();
    }

    // ----------------------------------------------------------------
    // Private Methods
    // ----------------------------------------------------------------

    /// <summary>
    /// Asks the user for their name and personalises the greeting.
    /// Includes input validation — will keep asking until a valid
    /// (non-empty) name is entered.
    /// </summary>
    private void GreetUser()
    {
        DisplayHelper.ShowSectionHeader("Welcome");

        DisplayHelper.BotSay("Welcome to the Cybersecurity Awareness Bot!");
        DisplayHelper.BotSay("Before we start, may I know your name?");

        DisplayHelper.UserPrompt();
        string? input = Console.ReadLine()?.Trim();

        // Input validation: keep prompting until a real name is given
        while (string.IsNullOrWhiteSpace(input))
        {
            DisplayHelper.ShowError("Please enter your name to continue.");
            DisplayHelper.UserPrompt();
            input = Console.ReadLine()?.Trim();
        }

        // Store the validated name using the auto-property
        UserName = input;

        DisplayHelper.ShowDivider();

        // Personalise the welcome with the user's name
        DisplayHelper.BotSay($"Great to meet you, {UserName}! 🎉");
        DisplayHelper.BotSay(
            "I'm here to help you understand cybersecurity threats " +
            "and how to protect yourself online.");
        DisplayHelper.BotSay(
            "Type 'help' to see all topics, or just ask me anything. " +
            "Type 'exit' when you're done.");

        DisplayHelper.ShowDivider();
    }

    /// <summary>
    /// The main conversation loop. Continuously reads user input,
    /// validates it, looks up a response, and displays it.
    /// Breaks out of the loop when the user types an exit command.
    /// </summary>
    private void RunConversationLoop()
    {
        DisplayHelper.ShowSectionHeader($"Chat Session — {UserName}");

        while (true)
        {
            // Prompt the user for input
            DisplayHelper.UserPrompt();
            string? input = Console.ReadLine()?.Trim();

            // ── Input Validation ──────────────────────────────────
            // Handle completely empty or whitespace-only input
            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayHelper.ShowError(
                    "You didn't type anything. " +
                    "Please enter a question or type 'help'.");
                continue; // Skip to the next iteration — don't crash
            }

            // Handle excessively long input (over 300 chars) — likely not valid
            if (input.Length > 300)
            {
                DisplayHelper.ShowError(
                    "That input is very long. " +
                    "Please keep your question shorter and try again.");
                continue;
            }

            // ── Increment Message Counter ─────────────────────────
            MessageCount++;

            // ── Check for Exit Commands ───────────────────────────
            if (ResponseEngine.IsExitCommand(input))
            {
                // Get the farewell response then personalise it
                string farewell = ResponseEngine.GetResponse(input);
                DisplayHelper.BotSay(farewell);
                DisplayHelper.BotSay(
                    $"You asked {MessageCount} question(s) today, {UserName}. " +
                    "Well done for taking your cybersecurity seriously!");
                DisplayHelper.ShowDivider();
                break; // Exit the loop and end the programme
            }

            // ── Get & Display Response ────────────────────────────
            string response = ResponseEngine.GetResponse(input);
            DisplayHelper.BotSay(response);
            DisplayHelper.ShowDivider();

            // Show a periodic tip every 5 messages to keep engagement up
            if (MessageCount % 5 == 0)
            {
                DisplayHelper.ShowTip(
                    $"You've asked {MessageCount} questions so far, {UserName}! " +
                    "Every question makes you more cyber-aware. 🛡️");
                DisplayHelper.ShowDivider();
            }
        }
    }
}
