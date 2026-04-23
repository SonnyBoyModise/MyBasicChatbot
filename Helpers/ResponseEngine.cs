// ============================================================
// ResponseEngine.cs
// Contains all the chatbot's knowledge as a keyword-to-response
// dictionary. GetResponse() scans the user's input for known
// keywords and returns the matching educational response.
// Falls back to a helpful default if no keyword is matched.
// ============================================================

namespace MyBasicChatbot.Helpers;

public static class ResponseEngine
{
    // ----------------------------------------------------------------
    // Response Dictionary
    // Key   = keyword the bot listens for (case-insensitive match)
    // Value = the educational response string shown to the user
    // ----------------------------------------------------------------
    private static readonly Dictionary<string, string> _responses =
        new(StringComparer.OrdinalIgnoreCase)
    {
        // ── General Conversation ────────────────────────────────────

        { "hello",
          "Hello! Great to have you here. I'm your Cybersecurity Awareness Bot. " +
          "Ask me anything about staying safe online!" },

        { "hi",
          "Hi there! Ready to learn about cybersecurity? " +
          "Type 'help' to see what topics I cover." },

        { "how are you",
          "I'm running at full security capacity — all firewalls up and no threats detected! 😄 " +
          "How can I help you stay safe today?" },

        { "what is your purpose",
          "My purpose is to educate South African citizens about online threats such as phishing, " +
          "weak passwords, malware, and social engineering — so you can protect yourself and " +
          "your family online." },

        { "what can i ask",
          "You can ask me about any of these topics:\n" +
          "     • Phishing & phishing emails\n" +
          "     • Password safety & two-factor authentication\n" +
          "     • Safe browsing & HTTPS\n" +
          "     • Malware & ransomware\n" +
          "     • Social engineering\n" +
          "     • South African cyber law\n" +
          "     • SARS scams\n" +
          "     • General online scams\n\n" +
          "  Just type any of those words and I'll explain!" },

        { "help",
          "Here are the topics I can help you with:\n" +
          "     • phishing          - fake emails & links\n" +
          "     • password          - how to create strong passwords\n" +
          "     • safe browsing     - staying safe on the web\n" +
          "     • malware           - viruses, spyware, ransomware\n" +
          "     • social engineering- psychological manipulation attacks\n" +
          "     • south africa      - local cyber threats & laws\n" +
          "     • sars scam         - common SA tax scams\n" +
          "     • two factor        - extra login security (2FA)\n" +
          "     • https             - what the padlock icon means\n\n" +
          "  Type 'exit' or 'bye' to leave the chat." },

        // ── Phishing ────────────────────────────────────────────────

        { "phishing",
          "Phishing is when cybercriminals send fake emails or messages pretending to be from " +
          "trusted organisations (like your bank, SARS, or FNB) to trick you into revealing " +
          "personal information or clicking malicious links.\n\n" +
          "  ✅ How to protect yourself:\n" +
          "     • Never click links in unexpected emails — go directly to the website\n" +
          "     • Check the sender's email address very carefully (e.g. sars@sars-gov.co vs sars.gov.za)\n" +
          "     • Look for spelling mistakes and urgency ('Your account will close in 24hrs!')\n" +
          "     • When in doubt, call the organisation on their official number\n" +
          "     • Report phishing emails to the South African Cybersecurity Hub" },

        { "phishing email",
          "A phishing email typically:\n" +
          "     • Creates false urgency ('Verify now or lose access!')\n" +
          "     • Contains spelling or grammar errors\n" +
          "     • Uses a sender address that looks almost right (e.g. support@paypa1.com)\n" +
          "     • Asks you to click a link or download an attachment\n" +
          "     • Requests personal info like passwords, ID numbers, or bank details\n\n" +
          "  ⚠️  Rule: Legitimate organisations will NEVER ask for your password via email." },

        // ── Passwords ───────────────────────────────────────────────

        { "password",
          "Strong passwords are your first line of defence against hackers!\n\n" +
          "  ✅ Best practices:\n" +
          "     • Use at least 12 characters — longer is better\n" +
          "     • Mix UPPERCASE, lowercase, numbers (123), and symbols (!@#)\n" +
          "     • Never reuse the same password across different websites\n" +
          "     • Avoid personal info: your name, ID number, or birthday\n" +
          "     • Use a password manager (e.g. Bitwarden — it's free!)\n" +
          "     • Enable Two-Factor Authentication (2FA) on all important accounts\n\n" +
          "  Example of a strong password: M@ngoes#River$2025!" },

        { "password safety",
          "Key password safety rules:\n" +
          "     • Change passwords immediately if you suspect a breach\n" +
          "     • Never share your password with anyone — not even IT support\n" +
          "     • Use a unique password for your email — it's the key to everything else\n" +
          "     • Check if your email has been breached at: haveibeenpwned.com" },

        { "two factor",
          "Two-Factor Authentication (2FA) adds a second layer of security to your accounts.\n\n" +
          "  How it works:\n" +
          "     1. You enter your password (something you KNOW)\n" +
          "     2. You confirm with a code sent to your phone (something you HAVE)\n\n" +
          "  Even if a hacker steals your password, they still cannot log in without " +
          "your phone. Always enable 2FA on:\n" +
          "     • Online banking\n" +
          "     • Email accounts\n" +
          "     • Social media\n" +
          "     • Any account with personal or financial information" },

        // ── Safe Browsing ───────────────────────────────────────────

        { "safe browsing",
          "Staying safe while browsing the internet:\n\n" +
          "  ✅ Key habits:\n" +
          "     • Always look for 'https://' and a 🔒 padlock before entering personal info\n" +
          "     • Avoid using public Wi-Fi (e.g. coffee shops) for banking or shopping\n" +
          "     • Keep your browser, plugins, and OS updated at all times\n" +
          "     • Install a reputable antivirus (e.g. Malwarebytes, Windows Defender)\n" +
          "     • Only download software from official sources\n" +
          "     • Be careful with pop-up ads — never click 'You've won a prize!'" },

        { "https",
          "HTTPS (the 🔒 padlock icon) means the data between your browser and " +
          "the website is encrypted in transit — no one in the middle can read it.\n\n" +
          "  ⚠️  Important: HTTPS does NOT mean the website is trustworthy or legitimate!\n" +
          "  Scam websites can also have HTTPS. Always verify the domain name carefully\n" +
          "  before entering any personal information." },

        // ── Malware ─────────────────────────────────────────────────

        { "malware",
          "Malware is malicious software designed to damage, disrupt, or steal data " +
          "from your device. Common types:\n\n" +
          "     • Virus       — spreads between files on your device\n" +
          "     • Ransomware  — encrypts your files and demands payment to restore them\n" +
          "     • Spyware     — secretly monitors your activity and steals info\n" +
          "     • Trojan      — disguised as legitimate software\n" +
          "     • Adware      — floods your screen with unwanted ads\n\n" +
          "  ✅ Protection:\n" +
          "     • Keep Windows and all software updated\n" +
          "     • Use antivirus software and run regular scans\n" +
          "     • Never download attachments from unknown senders" },

        { "ransomware",
          "Ransomware is one of the most dangerous cyber threats today. It encrypts " +
          "all your files and demands a payment (ransom) to give them back.\n\n" +
          "  🛡️  How to protect yourself:\n" +
          "     • Back up your important files REGULARLY — to an external drive or cloud\n" +
          "     • Never click suspicious email attachments or unknown links\n" +
          "     • Keep Windows and software fully updated\n" +
          "     • Never pay the ransom — it does not guarantee your files back\n\n" +
          "  📞 SA Businesses: Report attacks to CSIRT at cybersecurityhub.gov.za" },

        { "virus",
          "A computer virus is a type of malware that attaches itself to files and " +
          "spreads when those files are shared. Viruses can delete data, slow your " +
          "computer, or give attackers remote access.\n\n" +
          "  ✅ Prevention:\n" +
          "     • Run reputable antivirus software (Windows Defender is built-in)\n" +
          "     • Don't open email attachments you weren't expecting\n" +
          "     • Avoid pirated software — it often contains hidden malware" },

        // ── Social Engineering ───────────────────────────────────────

        { "social engineering",
          "Social engineering is when attackers manipulate people psychologically " +
          "to trick them into revealing information or granting access — without " +
          "needing to hack a system at all.\n\n" +
          "  Common techniques:\n" +
          "     • Pretexting   — creating a fake scenario to extract info\n" +
          "     • Baiting      — leaving a malicious USB drive for curiosity to do the rest\n" +
          "     • Vishing      — phone calls pretending to be IT support or your bank\n" +
          "     • Tailgating   — physically following someone into a secure area\n\n" +
          "  ✅ Defence:\n" +
          "     • Always verify a person's identity before sharing ANY information\n" +
          "     • Your bank will NEVER call and ask for your full PIN or password\n" +
          "     • When in doubt — hang up and call the organisation back officially" },

        // ── South Africa Specific ───────────────────────────────────

        { "south africa",
          "South Africa is among the top targeted countries for cybercrime on the continent.\n\n" +
          "  📋 Key resources for SA citizens:\n" +
          "     • Cybersecurity Hub: cybersecurityhub.gov.za\n" +
          "     • SAPS Cybercrime Unit: report online crime at www.saps.gov.za\n" +
          "     • Report online fraud to your bank's fraud hotline immediately\n\n" +
          "  ⚖️  The Cybercrimes Act No. 19 of 2020 criminalises:\n" +
          "     • Unauthorised access to systems\n" +
          "     • Data breaches and cyberbullying\n" +
          "     • Distributing malware and ransomware attacks\n" +
          "     • Online fraud and identity theft" },

        { "sars scam",
          "SARS (South African Revenue Service) phishing scams are extremely common.\n\n" +
          "  🚨 Remember these rules:\n" +
          "     • SARS will NEVER ask for your banking details via email or SMS\n" +
          "     • SARS will NEVER demand immediate payment via WhatsApp\n" +
          "     • Always verify SARS communication at the official site: www.sars.gov.za\n" +
          "     • Log in to your eFiling account directly — never via a link in an email\n\n" +
          "  📧 Report suspected SARS phishing to: phishing@sars.gov.za" },

        { "scam",
          "Online scams in South Africa are on the rise. Common ones include:\n" +
          "     • Advance-fee fraud ('Send R500 to claim your R50,000 prize')\n" +
          "     • Fake job offers asking for personal documents or a registration fee\n" +
          "     • Romance scams on dating apps or Facebook\n" +
          "     • Online shopping fraud — paying for goods that never arrive\n\n" +
          "  ✅ Rule of thumb: If something sounds too good to be true, it almost certainly is.\n" +
          "  Report scams at: www.cybersecurityhub.gov.za" },

        // ── Cyber Law ───────────────────────────────────────────────

        { "cyber law",
          "South Africa has several laws protecting citizens online:\n\n" +
          "     • Cybercrimes Act (2020)  — criminalises hacking, ransomware, cyberbullying\n" +
          "     • POPIA (2021)            — companies must protect your personal data\n" +
          "     • ECT Act                 — governs electronic communications & transactions\n\n" +
          "  If you are a victim of cybercrime, report it to the SAPS or your nearest " +
          "cybercrime unit. You can also contact the Cybersecurity Hub at cybersecurityhub.gov.za" },

        { "popia",
          "POPIA (Protection of Personal Information Act) came into full effect in 2021. " +
          "It requires all organisations in South Africa to:\n" +
          "     • Collect only the personal data they need\n" +
          "     • Store it securely and prevent data breaches\n" +
          "     • Inform you if your data is breached\n" +
          "     • Not sell your data without your consent\n\n" +
          "  As a citizen, you have the right to ask any company what data they hold about you " +
          "and to request it be deleted." },

        // ── Exit ─────────────────────────────────────────────────────

        { "bye",
          "Goodbye! Stay cyber-safe out there. Remember: think before you click! 🔐" },

        { "exit",
          "Exiting the Cybersecurity Awareness Bot. Stay vigilant and safe online! 👋" },

        { "quit",
          "Goodbye! Keep your passwords strong and your guard up. Until next time! 🛡️" },
    };

    // ----------------------------------------------------------------
    // Public Methods
    // ----------------------------------------------------------------

    /// <summary>
    /// Scans the user's input for any known keyword and returns
    /// the matching educational response. Returns a polite fallback
    /// message if no keyword is matched.
    /// </summary>
    /// <param name="input">Raw string typed by the user.</param>
    /// <returns>A string response to display to the user.</returns>
    public static string GetResponse(string input)
    {
        // Empty input is handled separately in ChatBot — but guard here too
        if (string.IsNullOrWhiteSpace(input))
        {
            return "It looks like you didn't type anything. " +
                   "Please ask me a cybersecurity question or type 'help'.";
        }

        // Search the dictionary for any keyword contained in the user's input
        foreach (var key in _responses.Keys)
        {
            if (input.Contains(key, StringComparison.OrdinalIgnoreCase))
            {
                return _responses[key];
            }
        }

        // No keyword matched — return a helpful default/fallback
        return "I didn't quite understand that. Could you rephrase?\n\n" +
               "  Try asking about: 'phishing', 'passwords', 'safe browsing',\n" +
               "  'malware', 'social engineering', 'sars scam', or type 'help'\n" +
               "  to see all available topics.";
    }

    /// <summary>
    /// Returns true if the user's input is an exit command (bye/exit/quit).
    /// Used by ChatBot to decide when to end the conversation loop.
    /// </summary>
    /// <param name="input">Raw string typed by the user.</param>
    /// <returns>True if the user wants to exit, false otherwise.</returns>
    public static bool IsExitCommand(string input)
    {
        return input.Equals("bye",  StringComparison.OrdinalIgnoreCase)
            || input.Equals("exit", StringComparison.OrdinalIgnoreCase)
            || input.Equals("quit", StringComparison.OrdinalIgnoreCase);
    }
}
