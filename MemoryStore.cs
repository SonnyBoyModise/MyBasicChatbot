// MemoryStore.cs — remembers things the user told us during the chat

namespace MyBasicChatbot.Helpers
{
    public class MemoryStore
    {
        // Generic Dictionary to store any key-value pair we want to remember
        private readonly Dictionary<string, string> _memory = new(StringComparer.OrdinalIgnoreCase);

        public string UserName  { get; set; } = string.Empty;
        public string LastTopic { get; set; } = string.Empty;
        public string Interest  { get; set; } = string.Empty;
        public string Concern   { get; set; } = string.Empty;
        public int    MessageCount { get; set; } = 0;

        public void Remember(string key, string value) => _memory[key] = value;

        public string Recall(string key) =>
            _memory.TryGetValue(key, out string? val) ? val : string.Empty;

        public bool Has(string key) => _memory.ContainsKey(key);

        public string GetPersonalisedPrefix()
        {
            if (!string.IsNullOrEmpty(Interest))
                return $"As someone interested in {Interest}, ";
            if (!string.IsNullOrEmpty(Concern))
                return $"Given your concern about {Concern}, ";
            return string.Empty;
        }
    }
}
