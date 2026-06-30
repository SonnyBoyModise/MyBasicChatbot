// QuizEngine.cs — manages all quiz questions, answers, and scoring

namespace MyBasicChatbot.Helpers
{
    // Holds one quiz question and its answer
    public class QuizQuestion
    {
        public string Text { get; set; } = "";
        public List<string> Options { get; set; } = new();
        public int CorrectIndex { get; set; }      // 0-based index of correct option
        public string Explanation { get; set; } = "";
        public bool IsTrueFalse { get; set; } = false;  // True = show True/False buttons
    }

    public class QuizEngine
    {
        // All quiz questions
        private readonly List<QuizQuestion> _questions;

        // Which question we are currently on
        private int _currentIndex = 0;

        // How many the user got right
        public int Score { get; private set; } = 0;

        // True when all questions have been answered
        public bool IsFinished => _currentIndex >= _questions.Count;

        // Which question number we are on (1-based for display)
        public int CurrentNumber => _currentIndex + 1;

        // Total number of questions
        public int TotalQuestions => _questions.Count;

        public QuizEngine()
        {
            _questions = BuildQuestions();
        }

        // Returns the current question
        public QuizQuestion GetCurrentQuestion()
        {
            if (IsFinished) return _questions[^1];
            return _questions[_currentIndex];
        }

        // Checks the user's answer — returns true if correct
        public bool SubmitAnswer(int selectedIndex)
        {
            var q = GetCurrentQuestion();
            bool correct = selectedIndex == q.CorrectIndex;
            if (correct) Score++;
            _currentIndex++;
            return correct;
        }

        // Moves on (called after showing feedback)
        public void MoveNext() => _currentIndex++;

        // Returns the question that was just answered (index - 1)
        public QuizQuestion? GetPreviousQuestion()
        {
            if (_currentIndex == 0) return null;
            return _questions[_currentIndex - 1];
        }

        // Resets the quiz so the user can play again
        public void Reset()
        {
            _currentIndex = 0;
            Score = 0;
        }

        // Returns the final result message based on score
        public string GetFinalMessage()
        {
            double percent = (double)Score / TotalQuestions * 100;

            if (percent >= 90)
                return $"🏆 Outstanding! {Score}/{TotalQuestions} — You're a cybersecurity pro!";
            if (percent >= 70)
                return $"🎉 Great job! {Score}/{TotalQuestions} — You know your stuff!";
            if (percent >= 50)
                return $"👍 Not bad! {Score}/{TotalQuestions} — Keep learning to stay safe!";

            return $"📚 Keep practising! {Score}/{TotalQuestions} — Review the topics and try again!";
        }

        // Builds the list of all 12 questions
        private static List<QuizQuestion> BuildQuestions()
        {
            return new List<QuizQuestion>
            {
                // Question 1 — Multiple choice
                new QuizQuestion
                {
                    Text = "What should you do if you receive an email asking for your password?",
                    Options = new() { "A) Reply with your password", "B) Delete the email", "C) Report it as phishing and delete it", "D) Forward it to friends" },
                    CorrectIndex = 2,
                    Explanation = "✅ Always report phishing emails — it helps protect others too."
                },

                // Question 2 — True/False
                new QuizQuestion
                {
                    Text = "True or False: HTTPS in a website address means the website is completely safe to use.",
                    Options = new() { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "✅ HTTPS only encrypts the connection. The site itself can still be a scam.",
                    IsTrueFalse = true
                },

                // Question 3 — Multiple choice
                new QuizQuestion
                {
                    Text = "Which of these is the strongest password?",
                    Options = new() { "A) password123", "B) MyName2000", "C) Tr@de$ecure#99!", "D) 12345678" },
                    CorrectIndex = 2,
                    Explanation = "✅ Strong passwords mix uppercase, lowercase, numbers, and symbols."
                },

                // Question 4 — Multiple choice
                new QuizQuestion
                {
                    Text = "What is phishing?",
                    Options = new() { "A) A type of computer virus", "B) Scammers pretending to be trusted sources to steal your info", "C) A method of backing up data", "D) A way to browse safely" },
                    CorrectIndex = 1,
                    Explanation = "✅ Phishing tricks you into revealing personal info by pretending to be someone you trust."
                },

                // Question 5 — True/False
                new QuizQuestion
                {
                    Text = "True or False: It is safe to use the same password for all your online accounts.",
                    Options = new() { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "✅ If one account is hacked, all your other accounts become vulnerable too.",
                    IsTrueFalse = true
                },

                // Question 6 — Multiple choice
                new QuizQuestion
                {
                    Text = "What does 2FA stand for?",
                    Options = new() { "A) Two Files Attached", "B) Two-Factor Authentication", "C) Twice Fixed Algorithm", "D) Two Firewall Access" },
                    CorrectIndex = 1,
                    Explanation = "✅ 2FA adds a second verification step, making accounts much harder to hack."
                },

                // Question 7 — Multiple choice
                new QuizQuestion
                {
                    Text = "What should you do if you find a USB drive in a public place?",
                    Options = new() { "A) Plug it in to see what's on it", "B) Leave it and report to lost and found", "C) Share it with friends", "D) Format it and use it" },
                    CorrectIndex = 1,
                    Explanation = "✅ Unknown USB drives can contain malware — never plug one in."
                },

                // Question 8 — True/False
                new QuizQuestion
                {
                    Text = "True or False: Ransomware encrypts your files and demands payment to restore them.",
                    Options = new() { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "✅ Correct! Regular backups are your best defence against ransomware.",
                    IsTrueFalse = true
                },

                // Question 9 — Multiple choice
                new QuizQuestion
                {
                    Text = "What is social engineering in cybersecurity?",
                    Options = new() { "A) Building social media platforms", "B) Manipulating people to reveal confidential information", "C) Engineering social networks", "D) Using AI to create content" },
                    CorrectIndex = 1,
                    Explanation = "✅ Social engineering exploits human trust rather than technical vulnerabilities."
                },

                // Question 10 — True/False
                new QuizQuestion
                {
                    Text = "True or False: It is safe to do online banking on public Wi-Fi.",
                    Options = new() { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "✅ Public Wi-Fi can be monitored by attackers. Use mobile data or a VPN for banking.",
                    IsTrueFalse = true
                },

                // Question 11 — Multiple choice
                new QuizQuestion
                {
                    Text = "Which is a common sign of a phishing email?",
                    Options = new() { "A) It comes from your boss's exact email", "B) It creates urgency: 'Your account closes in 24 hours!'", "C) It has your name correctly spelled", "D) It was sent during business hours" },
                    CorrectIndex = 1,
                    Explanation = "✅ Urgency is a classic tactic to pressure you into acting without thinking."
                },

                // Question 12 — Multiple choice
                new QuizQuestion
                {
                    Text = "What is the safest way to browse the internet?",
                    Options = new() { "A) Use any website that loads", "B) Only visit HTTPS sites and keep your browser updated", "C) Disable your antivirus for faster speeds", "D) Use the same tab for everything" },
                    CorrectIndex = 1,
                    Explanation = "✅ HTTPS and an updated browser protect you from many common threats."
                }
            };
        }
    }
}
