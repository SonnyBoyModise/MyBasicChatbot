// ============================================================
// Helpers/ResponseEngine.cs
// The brain of the chatbot. Handles:
//   - Keyword recognition for cybersecurity topics
//   - Random response selection from lists (using List<string>)
//   - Follow-up / "tell me more" conversation flow
//   - Memory-aware personalised responses
//   - Delegate for response post-processing
// ============================================================

namespace MyBasicChatbot.Helpers
{
    // ── Delegate for response post-processing ───────────────────────
    // Allows any method matching this signature to be plugged in to
    // transform a response string (e.g. add memory prefix, sentiment prefix)
    public delegate string ResponseProcessor(string response);

    public class ResponseEngine
    {
        // ── Random instance for selecting random responses ───────────
        private readonly Random _random = new();

        // ── Reference to memory so responses can be personalised ─────
        private readonly MemoryStore _memory;

        // Constructor — injects the shared MemoryStore
        public ResponseEngine(MemoryStore memory)
        {
            _memory = memory;
        }

        // ============================================================
        // RANDOM RESPONSE LISTS (List<string> generic collections)
        // Each list holds multiple responses for the same topic so
        // the bot doesn't always say the exact same thing.
        // ============================================================

        private readonly List<string> _phishingTips = new()
        {
            "🎣 Phishing Tip: Be cautious of emails asking for personal information. " +
            "Scammers often disguise themselves as trusted organisations like your bank or SARS.",

            "🎣 Phishing Tip: Check the sender's email address carefully. " +
            "Attackers use addresses like 'support@fnb-secure.co' instead of 'fnb.co.za'.",

            "🎣 Phishing Tip: Hover over links before clicking — the real URL shows " +
            "at the bottom of your browser. If it looks suspicious, don't click.",

            "🎣 Phishing Tip: Legitimate companies will NEVER ask for your password " +
            "or PIN via email or SMS. If they do, it's a scam.",

            "🎣 Phishing Tip: Watch out for urgency in emails — " +
            "'Your account will be closed in 24 hours!' is a classic pressure tactic."
        };

        private readonly List<string> _passwordTips = new()
        {
            "🔑 Password Tip: Use at least 12 characters mixing uppercase, lowercase, " +
            "numbers and symbols. Example: M@ngoes#River$2025!",

            "🔑 Password Tip: Never reuse the same password across different websites. " +
            "If one site is hacked, all your accounts stay safe.",

            "🔑 Password Tip: Use a password manager like Bitwarden (free!) to generate " +
            "and store strong, unique passwords for every site.",

            "🔑 Password Tip: Enable Two-Factor Authentication (2FA) on your email and " +
            "banking apps. Even if your password is stolen, hackers can't get in.",

            "🔑 Password Tip: Check if your email has been in a data breach at " +
            "haveibeenpwned.com — it's free and safe to use."
        };

        private readonly List<string> _malwareTips = new()
        {
            "🦠 Malware Tip: Keep Windows and all your software updated. " +
            "Most malware exploits known vulnerabilities in outdated software.",

            "🦠 Malware Tip: Never download software from unofficial websites. " +
            "Pirated software is one of the most common ways malware spreads in SA.",

            "🦠 Malware Tip: Don't plug in unknown USB drives — " +
            "attackers deliberately leave infected USBs in public places.",

            "🦠 Malware Tip: Windows Defender (built into Windows) is actually very effective. " +
            "Make sure it's enabled and up to date.",

            "🦠 Malware Tip: Back up your important files regularly to an external drive " +
            "or cloud storage. This is your best defence against ransomware."
        };

        private readonly List<string> _safeBrowsingTips = new()
        {
            "🌐 Browsing Tip: Always check for 'https://' and the padlock icon 🔒 " +
            "before entering any personal information on a website.",

            "🌐 Browsing Tip: Avoid doing online banking on public Wi-Fi. " +
            "Attackers can intercept your data on unsecured networks.",

            "🌐 Browsing Tip: Be careful of pop-up ads claiming you've won a prize. " +
            "These are almost always phishing attempts or malware installers.",

            "🌐 Browsing Tip: Use a reputable ad-blocker extension in your browser — " +
            "it also blocks many malicious ads that spread malware.",

            "🌐 Browsing Tip: Regularly clear your browser cookies and cache. " +
            "This reduces tracking and removes any stored malicious scripts."
        };

        // ============================================================
        // SINGLE RESPONSES (Dictionary for keyword lookup)
        // ============================================================

        private readonly Dictionary<string, string> _keywordResponses =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "hello",
              "Hello! 👋 I'm your Cybersecurity Awareness Bot. " +
              "Ask me anything about staying safe online, or click a topic button on the left!" },

            { "hi",
              "Hi there! Ready to learn about cybersecurity? " +
              "Try asking about phishing, passwords, malware, or safe browsing." },

            { "how are you",
              "I'm running at full security capacity — all firewalls up! 😄 " +
              "How can I help you stay safe online today?" },

            { "what is your purpose",
              "My purpose is to educate South African citizens about online threats " +
              "like phishing, weak passwords, and malware — so you and your family " +
              "can stay safer online." },

            { "help",
              "Here are the topics I can help you with:\n\n" +
              "• phishing / phishing tip\n" +
              "• password / password tip\n" +
              "• malware / malware tip\n" +
              "• ransomware\n" +
              "• safe browsing / browsing tip\n" +
              "• social engineering\n" +
              "• two factor / 2fa\n" +
              "• scam / sars scam\n" +
              "• south africa / cyber law / popia\n\n" +
              "Or click the quick topic buttons on the left! " +
              "You can also say 'tell me more' or 'give me another tip' to continue a topic." },

            { "two factor",
              "🔐 Two-Factor Authentication (2FA) adds a second layer of security.\n\n" +
              "How it works:\n" +
              "1. Enter your password (something you KNOW)\n" +
              "2. Confirm with an OTP sent to your phone (something you HAVE)\n\n" +
              "Even if someone steals your password, they cannot log in without your phone. " +
              "Always enable 2FA on your bank, email, and social media accounts." },

            { "2fa",
              "2FA (Two-Factor Authentication) is one of the most effective ways to " +
              "secure your accounts. Enable it on every account that offers it — " +
              "especially banking and email." },

            { "https",
              "🔒 HTTPS means the connection between your browser and the website is encrypted.\n\n" +
              "However — HTTPS does NOT mean the website is safe or legitimate! " +
              "Scam websites can also have HTTPS. Always verify the domain name carefully " +
              "before entering personal information." },

            { "ransomware",
              "💀 Ransomware encrypts all your files and demands payment to restore them.\n\n" +
              "Protection tips:\n" +
              "• Back up files regularly to an external drive or cloud\n" +
              "• Never click suspicious email attachments\n" +
              "• Keep Windows and software updated\n" +
              "• Never pay the ransom — it doesn't guarantee your files back\n\n" +
              "SA Businesses: Report attacks to cybersecurityhub.gov.za" },

            { "virus",
              "🦠 A computer virus attaches to files and spreads when shared. " +
              "Keep Windows Defender enabled and your OS updated to stay protected." },

            { "social engineering",
              "🎭 Social engineering attacks exploit human psychology rather than technology.\n\n" +
              "Common types:\n" +
              "• Pretexting — fake scenarios to extract info\n" +
              "• Vishing — phone calls pretending to be your bank or IT support\n" +
              "• Baiting — leaving infected USB drives in public\n" +
              "• Tailgating — following someone into a secure building\n\n" +
              "Rule: Your bank will NEVER call and ask for your full PIN or password." },

            { "south africa",
              "🇿🇦 South Africa is among the top targeted countries for cybercrime in Africa.\n\n" +
              "Key resources:\n" +
              "• Cybersecurity Hub: cybersecurityhub.gov.za\n" +
              "• Report online crime: www.saps.gov.za\n" +
              "• The Cybercrimes Act (2020) criminalises hacking, ransomware and online fraud." },

            { "sars scam",
              "⚠️ SARS phishing scams are extremely common in South Africa.\n\n" +
              "Remember:\n" +
              "• SARS will NEVER ask for banking details via email or SMS\n" +
              "• Always go directly to www.sars.gov.za — never via links in emails\n" +
              "• Report phishing to: phishing@sars.gov.za" },

            { "scam",
              "🚨 Common online scams in South Africa:\n\n" +
              "• Advance-fee fraud ('Send R500 to claim your R50,000 prize')\n" +
              "• Fake job offers asking for documents or a registration fee\n" +
              "• Romance scams on dating apps or Facebook\n" +
              "• Online shopping fraud — paying for goods that never arrive\n\n" +
              "Rule: If it sounds too good to be true — it is.\n" +
              "Report scams at: cybersecurityhub.gov.za" },

            { "cyber law",
              "⚖️ South African Cyber Laws:\n\n" +
              "• Cybercrimes Act (2020) — criminalises hacking, ransomware, cyberbullying\n" +
              "• POPIA (2021) — companies must protect your personal data\n" +
              "• ECT Act — governs electronic communications and transactions\n\n" +
              "Report cybercrime to the SAPS or cybersecurityhub.gov.za" },

            { "popia",
              "📋 POPIA (Protection of Personal Information Act) — in full effect since 2021.\n\n" +
              "Your rights:\n" +
              "• Companies must collect only data they need\n" +
              "• They must inform you if your data is breached\n" +
              "• You can ask any company what data they hold about you\n" +
              "• You can request your data be deleted" },

            { "privacy",
              "🔏 Protecting your privacy online:\n\n" +
              "• Review your social media privacy settings regularly\n" +
              "• Don't share your ID number, address or phone number publicly\n" +
              "• Use a VPN on public Wi-Fi to encrypt your connection\n" +
              "• Read app permissions — many apps request unnecessary access\n" +
              "• Under POPIA, companies must protect your personal information." },

            { "bye",      "Goodbye! Stay cyber-safe out there. Remember: think before you click! 🔐" },
            { "exit",     "Take care! Keep your passwords strong and your guard up. 👋" },
            { "thank",    "You're welcome! Stay safe online. 😊 Feel free to ask anything else." },
            { "thanks",   "Happy to help! Cybersecurity awareness is the first step to staying safe. 🛡️" },
        };

        // ============================================================
        // MAIN RESPONSE METHOD
        // ============================================================

        /// <summary>
        /// Main method called by the UI. Takes the user's raw input,
        /// detects intent and sentiment, builds the response, and
        /// uses the ResponseProcessor delegate to apply any final transforms.
        /// </summary>
        public string GetResponse(string input, ResponseProcessor? processor = null)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "It looks like you didn't type anything. " +
                       "Please ask me a cybersecurity question or click a topic on the left!";

            string lower = input.ToLower().Trim();

            // ── Update memory: increment message count ───────────────
            _memory.MessageCount++;

            // ── Detect and store interest from input ─────────────────
            DetectAndStoreInterest(lower);

            // ── Build the core response ──────────────────────────────
            string response = BuildResponse(lower);

            // ── Apply the delegate processor if one was passed ───────
            // This allows the caller (MainWindow) to inject sentiment
            // prefix and memory prefix without the engine knowing about UI
            if (processor != null)
                response = processor(response);

            return response;
        }

        /// <summary>
        /// Core response logic: checks for follow-up commands first,
        /// then random tip requests, then keyword dictionary matches,
        /// then falls back to a default message.
        /// </summary>
        private string BuildResponse(string lower)
        {
            // ── 1. Follow-up / conversation flow commands ────────────
            if (IsFollowUp(lower))
                return HandleFollowUp();

            // ── 2. Random tip requests ───────────────────────────────
            if (lower.Contains("phishing tip") || lower.Contains("phishing advice"))
                return GetRandom(_phishingTips);

            if (lower.Contains("password tip") || lower.Contains("password advice"))
                return GetRandom(_passwordTips);

            if (lower.Contains("malware tip") || lower.Contains("malware advice"))
                return GetRandom(_malwareTips);

            if (lower.Contains("browsing tip") || lower.Contains("safe browsing tip"))
                return GetRandom(_safeBrowsingTips);

            // ── 3. Keyword dictionary lookup ─────────────────────────
            foreach (var key in _keywordResponses.Keys)
            {
                if (lower.Contains(key))
                {
                    // Remember the last topic discussed
                    _memory.LastTopic = key;
                    _memory.Remember("lastTopic", key);
                    return _keywordResponses[key];
                }
            }

            // ── 4. Topic keywords with random tip selection ──────────
            if (lower.Contains("phishing"))
            {
                _memory.LastTopic = "phishing";
                return GetRandom(_phishingTips);
            }

            if (lower.Contains("password"))
            {
                _memory.LastTopic = "password";
                return GetRandom(_passwordTips);
            }

            if (lower.Contains("malware") || lower.Contains("virus"))
            {
                _memory.LastTopic = "malware";
                return GetRandom(_malwareTips);
            }

            if (lower.Contains("browsing") || lower.Contains("browse"))
            {
                _memory.LastTopic = "safe browsing";
                return GetRandom(_safeBrowsingTips);
            }

            // ── 5. Default fallback ──────────────────────────────────
            return "I didn't quite understand that — could you rephrase? 🤔\n\n" +
                   "Try asking about: phishing, passwords, malware, safe browsing, " +
                   "social engineering, scams, or SA cyber law.\n" +
                   "Or click one of the quick topic buttons on the left!";
        }

        // ============================================================
        // FOLLOW-UP / CONVERSATION FLOW
        // ============================================================

        /// <summary>
        /// Returns true if the user's message is a follow-up request
        /// like "tell me more", "give me another tip", "explain more".
        /// </summary>
        public bool IsFollowUp(string lower)
        {
            List<string> followUpPhrases = new()
            {
                "tell me more", "more info", "explain more", "more details",
                "give me another", "another tip", "give me a tip",
                "go on", "continue", "what else", "more please",
                "elaborate", "expand on that", "keep going"
            };

            foreach (string phrase in followUpPhrases)
            {
                if (lower.Contains(phrase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Handles follow-up requests by giving another tip on the last topic.
        /// </summary>
        private string HandleFollowUp()
        {
            string lastTopic = _memory.LastTopic.ToLower();

            if (lastTopic.Contains("phishing"))
                return "Here's another phishing tip:\n\n" + GetRandom(_phishingTips);

            if (lastTopic.Contains("password"))
                return "Here's another password safety tip:\n\n" + GetRandom(_passwordTips);

            if (lastTopic.Contains("malware") || lastTopic.Contains("virus"))
                return "Here's another malware tip:\n\n" + GetRandom(_malwareTips);

            if (lastTopic.Contains("browsing"))
                return "Here's another safe browsing tip:\n\n" + GetRandom(_safeBrowsingTips);

            // No last topic — give a general tip
            return "Here's a general cybersecurity tip:\n\n" +
                   GetRandom(_phishingTips) + "\n\n" +
                   "Ask me about a specific topic for more detailed advice!";
        }

        // ============================================================
        // INTEREST / MEMORY DETECTION
        // ============================================================

        /// <summary>
        /// Scans the user's input for expressions of interest or concern
        /// and stores them in the MemoryStore for personalised future replies.
        /// </summary>
        private void DetectAndStoreInterest(string lower)
        {
            // Detect interest expressions like "I'm interested in privacy"
            string[] interestPhrases = { "interested in", "care about", "want to learn about", "curious about" };
            foreach (string phrase in interestPhrases)
            {
                if (lower.Contains(phrase))
                {
                    // Extract the topic after the phrase
                    int idx = lower.IndexOf(phrase) + phrase.Length;
                    string topic = lower.Substring(idx).Trim().TrimEnd('.', '!', '?');
                    if (!string.IsNullOrEmpty(topic))
                    {
                        _memory.Interest = topic;
                        _memory.Remember("interest", topic);
                    }
                    return;
                }
            }

            // Detect concern expressions like "I'm worried about scams"
            string[] concernPhrases = { "worried about", "concerned about", "scared of", "afraid of" };
            foreach (string phrase in concernPhrases)
            {
                if (lower.Contains(phrase))
                {
                    int idx = lower.IndexOf(phrase) + phrase.Length;
                    string concern = lower.Substring(idx).Trim().TrimEnd('.', '!', '?');
                    if (!string.IsNullOrEmpty(concern))
                    {
                        _memory.Concern = concern;
                        _memory.Remember("concern", concern);
                    }
                    return;
                }
            }
        }

        // ============================================================
        // HELPERS
        // ============================================================

        /// <summary>
        /// Picks a random item from a List<string>.
        /// This is the core of the "random responses" feature.
        /// </summary>
        private string GetRandom(List<string> options)
        {
            if (options == null || options.Count == 0)
                return "I don't have a tip on that right now. Try asking something else!";

            return options[_random.Next(options.Count)];
        }

        /// <summary>
        /// Returns true if the input is an exit command.
        /// </summary>
        public bool IsExitCommand(string input)
        {
            return input.Equals("bye",  StringComparison.OrdinalIgnoreCase) ||
                   input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                   input.Equals("quit", StringComparison.OrdinalIgnoreCase);
        }
    }
}
