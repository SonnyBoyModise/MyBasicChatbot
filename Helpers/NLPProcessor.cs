// NLPProcessor.cs — figures out what the user wants to do (their intent)
// Uses keyword detection (string.Contains) to simulate basic NLP

using System.Text.RegularExpressions;

namespace MyBasicChatbot.Helpers
{
    // All the different things a user might want to do
    public enum UserIntent
    {
        Chat,            // Normal cybersecurity chat
        AddTask,         // Add a new task
        ViewTasks,       // See all tasks
        DeleteTask,      // Delete a task
        CompleteTask,    // Mark a task as done
        StartQuiz,       // Launch the quiz
        ShowActivityLog  // Show the activity log
    }

    public static class NLPProcessor
    {
        // Detects the user's intent from their message
        public static UserIntent DetectIntent(string input)
        {
            string lower = input.ToLower().Trim();

            // Task creation phrases
            if (ContainsAny(lower, new[]
                { "add task", "new task", "create task", "add a task",
                  "remind me to", "set a reminder", "add reminder",
                  "i need to", "i should", "don't let me forget" }))
                return UserIntent.AddTask;

            // View tasks phrases
            if (ContainsAny(lower, new[]
                { "view tasks", "show tasks", "my tasks", "list tasks",
                  "see tasks", "what tasks", "show my tasks" }))
                return UserIntent.ViewTasks;

            // Mark task complete phrases
            if (ContainsAny(lower, new[]
                { "complete task", "mark done", "mark complete",
                  "finish task", "task done", "i completed" }))
                return UserIntent.CompleteTask;

            // Delete task phrases
            if (ContainsAny(lower, new[]
                { "delete task", "remove task", "cancel task", "get rid of task" }))
                return UserIntent.DeleteTask;

            // Quiz phrases
            if (ContainsAny(lower, new[]
                { "start quiz", "play quiz", "take quiz", "quiz me",
                  "test me", "test my knowledge", "start game", "play game" }))
                return UserIntent.StartQuiz;

            // Activity log phrases
            if (ContainsAny(lower, new[]
                { "show log", "activity log", "what have you done",
                  "recent actions", "show history", "what did you do",
                  "show activity" }))
                return UserIntent.ShowActivityLog;

            return UserIntent.Chat;
        }

        // Tries to extract a task title from the user's message
        // e.g. "add task Enable 2FA" → "Enable 2FA"
        public static string ExtractTaskTitle(string input)
        {
            string lower = input.ToLower();

            // Try to strip the command prefix and return the rest as the title
            string[] prefixes = {
                "add a task to", "add task to", "add a task",
                "create a task to", "create task", "new task",
                "remind me to", "add reminder to", "add a reminder to",
                "i need to", "i should", "don't let me forget to"
            };

            // Sort longest first so we match the most specific prefix
            foreach (var prefix in prefixes.OrderByDescending(p => p.Length))
            {
                if (lower.Contains(prefix))
                {
                    int idx = lower.IndexOf(prefix) + prefix.Length;
                    if (idx < input.Length)
                        return input.Substring(idx).Trim().TrimEnd('.', '!', '?');
                }
            }

            return input.Trim();
        }

        // Tries to extract a number of days from the message
        // e.g. "in 3 days" → 3, "tomorrow" → 1, "next week" → 7
        public static int? ExtractDays(string input)
        {
            string lower = input.ToLower();

            if (lower.Contains("tomorrow"))   return 1;
            if (lower.Contains("next week"))  return 7;
            if (lower.Contains("next month")) return 30;

            // Match "in X days" or "in X day"
            var match = Regex.Match(lower, @"in (\d+) days?");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
                return days;

            return null;
        }

        // Helper — checks if input contains any of the given phrases
        private static bool ContainsAny(string input, string[] phrases)
        {
            foreach (var phrase in phrases)
                if (input.Contains(phrase)) return true;
            return false;
        }
    }
}
