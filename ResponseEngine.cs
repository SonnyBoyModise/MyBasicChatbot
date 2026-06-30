// ResponseEngine.cs — keyword recognition and random responses (Parts 1, 2 & 3)

namespace MyBasicChatbot.Helpers
{
    public delegate string ResponseProcessor(string response);

    public class ResponseEngine
    {
        private readonly Random _random = new();
        private readonly MemoryStore _memory;

        public ResponseEngine(MemoryStore memory) { _memory = memory; }

        private readonly List<string> _phishingTips = new()
        {
            "Phishing Tip: Be cautious of emails asking for personal info. Scammers disguise themselves as banks or SARS.",
            "Phishing Tip: Check the sender's address carefully — attackers use 'fnb-secure.co' instead of 'fnb.co.za'.",
            "Phishing Tip: Hover over links before clicking to see the real URL at the bottom of your browser.",
            "Phishing Tip: Legitimate companies will NEVER ask for your password via email or SMS.",
            "Phishing Tip: Urgency like 'Your account closes in 24 hours!' is a classic pressure tactic."
        };

        private readonly List<string> _passwordTips = new()
        {
            "Password Tip: Use at least 12 characters mixing uppercase, lowercase, numbers, and symbols.",
            "Password Tip: Never reuse the same password across different websites.",
            "Password Tip: Use a free password manager like Bitwarden to generate strong passwords.",
            "Password Tip: Enable Two-Factor Authentication (2FA) on your email and banking apps.",
            "Password Tip: Check if your email was in a breach at haveibeenpwned.com."
        };

        private readonly List<string> _malwareTips = new()
        {
            "Malware Tip: Keep Windows and all software updated — most malware exploits outdated software.",
            "Malware Tip: Never download software from unofficial websites.",
            "Malware Tip: Do not plug in unknown USB drives — attackers leave infected ones in public places.",
            "Malware Tip: Windows Defender is built-in and effective — keep it enabled and updated.",
            "Malware Tip: Back up important files regularly — your best defence against ransomware."
        };

        private readonly List<string> _browsingTips = new()
        {
            "Browsing Tip: Always check for 'https://' and the padlock before entering personal info.",
            "Browsing Tip: Avoid online banking on public Wi-Fi — attackers can intercept your data.",
            "Browsing Tip: 'You have won a prize!' pop-ups are always scams.",
            "Browsing Tip: Use an ad-blocker extension — it also blocks many malicious ads.",
            "Browsing Tip: Clear your browser cookies regularly to reduce tracking."
        };

        private readonly Dictionary<string, string> _responses = new(StringComparer.OrdinalIgnoreCase)
        {
            { "hello",               "Hello! Ask me about cybersecurity, or click a topic button on the left!" },
            { "hi",                  "Hi there! Try asking about phishing, passwords, malware, or safe browsing." },
            { "how are you",         "Running at full security capacity! How can I help you stay safe today?" },
            { "what is your purpose","I educate South African citizens about online threats like phishing and malware." },
            { "help",
              "Here is what I can do:\n\nChat: Ask about any cybersecurity topic\nTasks: Manage your cybersecurity to-do list\nQuiz: Test your cybersecurity knowledge\n\n" +
              "Try typing: phishing | password | malware | safe browsing | social engineering | scam | cyber law\n" +
              "Or try: add task | view tasks | start quiz | show activity log" },
            { "two factor",          "2FA adds a second login step. Even if someone steals your password, they cannot get in without your phone. Enable it on banking and email!" },
            { "2fa",                 "Enable 2FA on every account that offers it, especially banking and email." },
            { "https",               "HTTPS encrypts your connection but does not mean the site is safe. Scam sites can also use HTTPS. Always check the domain name carefully." },
            { "ransomware",          "Ransomware encrypts your files and demands payment. Protect yourself: back up files regularly, never click suspicious attachments, keep Windows updated." },
            { "social engineering",  "Social engineering manipulates people psychologically. Your bank will NEVER call and ask for your full PIN or password." },
            { "virus",               "Keep Windows Defender enabled and your OS updated to stay protected from viruses." },
            { "south africa",        "SA is a top target for cybercrime in Africa. Report online crime at cybersecurityhub.gov.za or www.saps.gov.za." },
            { "sars scam",           "SARS will NEVER ask for banking details via email or SMS. Go directly to www.sars.gov.za. Report phishing to phishing@sars.gov.za" },
            { "scam",                "Common SA scams: advance-fee fraud, fake job offers, romance scams, fake online shopping. If it sounds too good to be true, it is!" },
            { "cyber law",           "The Cybercrimes Act (2020) criminalises hacking, ransomware, and cyberbullying in SA. POPIA (2021) protects your personal data." },
            { "popia",               "POPIA gives you the right to know what data companies hold about you and to request it be deleted." },
            { "privacy",             "Review social media privacy settings, do not share your ID number publicly, and use a VPN on public Wi-Fi." },
            { "thank",               "You are welcome! Stay safe online." },
            { "thanks",              "Happy to help! Cybersecurity awareness is the first step to staying safe." },
            { "bye",                 "Goodbye! Stay cyber-safe. Remember: think before you click!" },
        };

        public string GetResponse(string input, ResponseProcessor? processor = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type a message or click a topic button!";

            string lower = input.ToLower().Trim();
            _memory.MessageCount++;
            DetectInterest(lower);

            string response = BuildResponse(lower);

            if (processor != null)
                response = processor(response);

            return response;
        }

        private string BuildResponse(string lower)
        {
            if (IsFollowUp(lower)) return HandleFollowUp();

            if (lower.Contains("phishing tip"))  return GetRandom(_phishingTips);
            if (lower.Contains("password tip"))  return GetRandom(_passwordTips);
            if (lower.Contains("malware tip"))   return GetRandom(_malwareTips);
            if (lower.Contains("browsing tip"))  return GetRandom(_browsingTips);

            foreach (var key in _responses.Keys)
            {
                if (lower.Contains(key))
                {
                    _memory.LastTopic = key;
                    return _responses[key];
                }
            }

            if (lower.Contains("phishing")) { _memory.LastTopic = "phishing"; return GetRandom(_phishingTips); }
            if (lower.Contains("password")) { _memory.LastTopic = "password"; return GetRandom(_passwordTips); }
            if (lower.Contains("malware") || lower.Contains("virus")) { _memory.LastTopic = "malware"; return GetRandom(_malwareTips); }
            if (lower.Contains("browsing") || lower.Contains("browse")) { _memory.LastTopic = "safe browsing"; return GetRandom(_browsingTips); }

            return "I did not quite understand that. Try asking about phishing, passwords, malware, safe browsing, or type 'help'.";
        }

        public bool IsFollowUp(string lower)
        {
            string[] phrases = { "tell me more", "more info", "explain more", "give me another", "another tip", "go on", "continue", "what else", "more please" };
            foreach (var p in phrases)
                if (lower.Contains(p)) return true;
            return false;
        }

        private string HandleFollowUp()
        {
            string topic = _memory.LastTopic.ToLower();
            if (topic.Contains("phishing")) return "Another phishing tip:\n\n" + GetRandom(_phishingTips);
            if (topic.Contains("password")) return "Another password tip:\n\n" + GetRandom(_passwordTips);
            if (topic.Contains("malware") || topic.Contains("virus")) return "Another malware tip:\n\n" + GetRandom(_malwareTips);
            if (topic.Contains("browsing")) return "Another browsing tip:\n\n" + GetRandom(_browsingTips);
            return "Here is a general tip:\n\n" + GetRandom(_phishingTips);
        }

        private void DetectInterest(string lower)
        {
            foreach (var phrase in new[] { "interested in", "curious about", "care about" })
            {
                if (lower.Contains(phrase))
                {
                    int idx = lower.IndexOf(phrase) + phrase.Length;
                    if (idx < lower.Length)
                    {
                        _memory.Interest = lower.Substring(idx).Trim().TrimEnd('.', '!', '?');
                        _memory.Remember("interest", _memory.Interest);
                    }
                    return;
                }
            }
            foreach (var phrase in new[] { "worried about", "concerned about", "scared of" })
            {
                if (lower.Contains(phrase))
                {
                    int idx = lower.IndexOf(phrase) + phrase.Length;
                    if (idx < lower.Length)
                    {
                        _memory.Concern = lower.Substring(idx).Trim().TrimEnd('.', '!', '?');
                        _memory.Remember("concern", _memory.Concern);
                    }
                    return;
                }
            }
        }

        private string GetRandom(List<string> options) => options[_random.Next(options.Count)];
    }
}
