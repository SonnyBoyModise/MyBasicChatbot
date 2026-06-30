// SentimentDetector.cs — detects the user's mood from their message

namespace MyBasicChatbot.Helpers
{
    public enum Sentiment { Neutral, Worried, Frustrated, Curious, Happy, Angry }

    public static class SentimentDetector
    {
        private static readonly List<string> WorriedWords    = new() { "worried","scared","afraid","nervous","anxious","terrified","fear","unsafe","concerned","dangerous","panic","stressed" };
        private static readonly List<string> FrustratedWords = new() { "frustrated","confused","dont understand","don't understand","not working","difficult","hard","complicated","lost","stuck","annoyed" };
        private static readonly List<string> CuriousWords    = new() { "curious","wondering","interesting","tell me more","how does","what is","explain","i want to know","teach me","learn","understand" };
        private static readonly List<string> HappyWords      = new() { "great","awesome","thanks","thank you","helpful","love it","amazing","excellent","perfect","good","nice","cool","fantastic" };
        private static readonly List<string> AngryWords      = new() { "angry","hate","useless","stupid","terrible","awful","worst","horrible","ridiculous","pathetic","rubbish","trash","nonsense" };

        public static Sentiment Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Sentiment.Neutral;
            string lower = input.ToLower();
            if (Has(lower, WorriedWords))    return Sentiment.Worried;
            if (Has(lower, AngryWords))      return Sentiment.Angry;
            if (Has(lower, FrustratedWords)) return Sentiment.Frustrated;
            if (Has(lower, CuriousWords))    return Sentiment.Curious;
            if (Has(lower, HappyWords))      return Sentiment.Happy;
            return Sentiment.Neutral;
        }

        public static string GetSentimentLabel(Sentiment s) => s switch
        {
            Sentiment.Worried    => "😟 Mood: Worried",
            Sentiment.Frustrated => "😤 Mood: Frustrated",
            Sentiment.Curious    => "🤔 Mood: Curious",
            Sentiment.Happy      => "😊 Mood: Happy",
            Sentiment.Angry      => "😠 Mood: Upset",
            _                    => "😐 Mood: Neutral"
        };

        public static string GetSentimentResponse(Sentiment s) => s switch
        {
            Sentiment.Worried    => "It's completely understandable to feel that way. Let me help put your mind at ease. ",
            Sentiment.Frustrated => "I hear you — let me break this down simply. ",
            Sentiment.Curious    => "Great question! Love the curiosity. ",
            Sentiment.Happy      => "Glad to hear that! ",
            Sentiment.Angry      => "I'm sorry you feel that way. Let me try to help: ",
            _                    => string.Empty
        };

        private static bool Has(string input, List<string> words)
        {
            foreach (var w in words)
                if (input.Contains(w)) return true;
            return false;
        }
    }
}
