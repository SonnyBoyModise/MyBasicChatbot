// ActivityLog.cs — keeps a record of everything the chatbot has done

namespace MyBasicChatbot.Helpers
{
    public class ActivityLog
    {
        // Stores log entries as strings — uses a generic List
        private readonly List<string> _entries = new();

        // How many recent entries to show when user asks for the log
        private const int MaxDisplay = 10;

        // Adds a new entry with a timestamp
        public void Add(string action)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            _entries.Add($"[{timestamp}] {action}");
        }

        // Returns the last 10 entries as a numbered list string
        public string GetLog()
        {
            if (_entries.Count == 0)
                return "No actions recorded yet. Try adding a task or taking the quiz!";

            // Take the most recent entries
            var recent = _entries.TakeLast(MaxDisplay).ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("📋 Recent Activity Log:\n");

            for (int i = 0; i < recent.Count; i++)
            {
                sb.AppendLine($"  {i + 1}. {recent[i]}");
            }

            return sb.ToString().TrimEnd();
        }

        // Returns how many entries are stored
        public int Count => _entries.Count;
    }
}
