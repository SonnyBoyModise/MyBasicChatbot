// ============================================================
// Helpers/SentimentDetector.cs
// Detects the emotional tone/sentiment of the user's message.
// Looks for keywords indicating worry, frustration, curiosity,
// happiness, or anger and returns a Sentiment enum value.
// The chatbot uses this to adjust its response tone.
// ============================================================

namespace MyBasicChatbot.Helpers
{
    // Enum representing the detected sentiment categories
    public enum Sentiment
    {
        Neutral,    // No strong emotion detected
        Worried,    // User seems anxious or concerned
        Frustrated, // User seems annoyed or stuck
        Curious,    // User seems interested and inquisitive
        Happy,      // User seems positive and upbeat
        Angry       // User seems upset or aggressive
    }

    public static class SentimentDetector
    {
        // ── Keyword lists for each sentiment ────────────────────────
        // Uses List<string> as required (generic collection)

        private static readonly List<string> WorriedKeywords = new()
        {
            "worried", "scared", "afraid", "nervous", "anxious",
            "terrified", "fear", "unsafe", "concern", "concerned",
            "not safe", "dangerous", "danger", "panic", "stressed"
        };

        private static readonly List<string> FrustratedKeywords = new()
        {
            "frustrated", "confused", "dont understand", "don't understand",
            "not working", "difficult", "hard", "complicated", "lost",
            "stuck", "annoyed", "irritated", "makes no sense", "ugh"
        };

        private static readonly List<string> CuriousKeywords = new()
        {
            "curious", "wondering", "interesting", "tell me more",
            "how does", "what is", "explain", "i want to know",
            "teach me", "learn", "understand", "fascinated", "intrigued"
        };

        private static readonly List<string> HappyKeywords = new()
        {
            "great", "awesome", "thanks", "thank you", "helpful",
            "love it", "amazing", "excellent", "perfect", "good",
            "nice", "cool", "fantastic", "wonderful", "appreciate"
        };

        private static readonly List<string> AngryKeywords = new()
        {
            "angry", "hate", "useless", "stupid", "terrible",
            "awful", "worst", "horrible", "disgusting", "ridiculous",
            "pathetic", "rubbish", "trash", "idiot", "nonsense"
        };

        // ── Public Detection Method ──────────────────────────────────

        /// <summary>
        /// Analyses the input string and returns the detected Sentiment.
        /// Checks each sentiment keyword list in priority order.
        /// </summary>
        /// <param name="input">The user's raw message text.</param>
        /// <returns>A Sentiment enum value representing the detected mood.</returns>
        public static Sentiment Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Sentiment.Neutral;

            string lower = input.ToLower();

            // Check each category — worried first as it's highest priority
            if (ContainsAny(lower, WorriedKeywords))     return Sentiment.Worried;
            if (ContainsAny(lower, AngryKeywords))       return Sentiment.Angry;
            if (ContainsAny(lower, FrustratedKeywords))  return Sentiment.Frustrated;
            if (ContainsAny(lower, CuriousKeywords))     return Sentiment.Curious;
            if (ContainsAny(lower, HappyKeywords))       return Sentiment.Happy;

            return Sentiment.Neutral;
        }

        /// <summary>
        /// Returns an emoji + label string for display in the UI sidebar.
        /// </summary>
        public static string GetSentimentLabel(Sentiment sentiment)
        {
            return sentiment switch
            {
                Sentiment.Worried    => "😟 Mood: Worried",
                Sentiment.Frustrated => "😤 Mood: Frustrated",
                Sentiment.Curious    => "🤔 Mood: Curious",
                Sentiment.Happy      => "😊 Mood: Happy",
                Sentiment.Angry      => "😠 Mood: Upset",
                _                    => "😐 Mood: Neutral"
            };
        }

        /// <summary>
        /// Returns a sentiment-appropriate opening line the bot prepends
        /// to its response to show empathy and adjust tone.
        /// </summary>
        public static string GetSentimentResponse(Sentiment sentiment)
        {
            return sentiment switch
            {
                Sentiment.Worried =>
                    "It's completely understandable to feel that way — " +
                    "cybersecurity threats can be scary. Let me help put your mind at ease. ",

                Sentiment.Frustrated =>
                    "I hear you — this stuff can feel overwhelming at first. " +
                    "Let me break it down simply for you. ",

                Sentiment.Curious =>
                    "Great question! I love the curiosity — " +
                    "that's exactly the right mindset for staying safe online. ",

                Sentiment.Happy =>
                    "Glad to hear that! Let's keep the good energy going. ",

                Sentiment.Angry =>
                    "I'm sorry you're feeling that way. " +
                    "Let me try to be more helpful — here's what I know: ",

                _ => string.Empty // Neutral — no prefix needed
            };
        }

        // ── Private Helper ───────────────────────────────────────────

        /// <summary>
        /// Returns true if the input string contains any keyword from the list.
        /// </summary>
        private static bool ContainsAny(string input, List<string> keywords)
        {
            foreach (string keyword in keywords)
            {
                if (input.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
