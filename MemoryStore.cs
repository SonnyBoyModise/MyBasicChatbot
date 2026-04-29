// ============================================================
// Helpers/MemoryStore.cs
// Implements the chatbot's "memory" feature.
// Stores user details gathered during conversation and
// exposes them so the bot can personalise future responses.
// Uses a generic Dictionary<string, string> as required
// by the assignment (generic collection).
// ============================================================

namespace MyBasicChatbot.Helpers
{
    public class MemoryStore
    {
        // ── Generic Dictionary to store all memory key-value pairs ──
        // Key   = what we're remembering (e.g. "interest", "concern")
        // Value = what the user said (e.g. "phishing", "online scams")
        private readonly Dictionary<string, string> _memory = new(StringComparer.OrdinalIgnoreCase);

        // ── Auto-properties for quick access to common fields ────────

        /// <summary>The user's name, set during the welcome screen.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>The cybersecurity topic the user last asked about.</summary>
        public string LastTopic { get; set; } = string.Empty;

        /// <summary>The topic the user expressed interest in (e.g. "privacy").</summary>
        public string Interest { get; set; } = string.Empty;

        /// <summary>A concern the user mentioned (e.g. "worried about scams").</summary>
        public string Concern { get; set; } = string.Empty;

        /// <summary>Total messages sent in this session.</summary>
        public int MessageCount { get; set; } = 0;

        // ── Generic Dictionary Methods ───────────────────────────────

        /// <summary>
        /// Stores any key-value pair in memory (uses the generic Dictionary).
        /// </summary>
        public void Remember(string key, string value)
        {
            // Update if key exists, add if it doesn't
            _memory[key] = value;
        }

        /// <summary>
        /// Retrieves a stored memory value by key.
        /// Returns empty string if not found.
        /// </summary>
        public string Recall(string key)
        {
            return _memory.TryGetValue(key, out string? value) ? value : string.Empty;
        }

        /// <summary>
        /// Returns true if a specific key has been stored in memory.
        /// </summary>
        public bool Has(string key)
        {
            return _memory.ContainsKey(key);
        }

        /// <summary>
        /// Clears all memory (used when session resets).
        /// </summary>
        public void Clear()
        {
            _memory.Clear();
            UserName     = string.Empty;
            LastTopic    = string.Empty;
            Interest     = string.Empty;
            Concern      = string.Empty;
            MessageCount = 0;
        }

        /// <summary>
        /// Builds a personalised prefix for bot messages using stored memory.
        /// Returns an empty string if there's nothing relevant to reference.
        /// </summary>
        public string GetPersonalisedPrefix()
        {
            // Reference the user's interest if we have one
            if (!string.IsNullOrEmpty(Interest))
                return $"As someone interested in {Interest}, ";

            // Reference a concern they mentioned
            if (!string.IsNullOrEmpty(Concern))
                return $"Given your concern about {Concern}, ";

            return string.Empty;
        }
    }
}
