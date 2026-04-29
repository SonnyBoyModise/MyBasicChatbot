// ============================================================
// MainWindow.xaml.cs
// Code-behind for the main WPF window.
// Handles:
//   - Name entry screen and validation
//   - Sending and displaying chat messages
//   - Calling the ResponseEngine with a delegate processor
//   - Updating the sidebar (memory panel, sentiment label)
//   - Quick topic button clicks
//   - Rotating "Did You Know" tips
//   - Voice greeting on startup
// ============================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MyBasicChatbot.Helpers;

namespace MyBasicChatbot
{
    public partial class MainWindow : Window
    {
        // ── Core components ──────────────────────────────────────────
        private readonly MemoryStore     _memory;
        private readonly ResponseEngine  _engine;

        // ── Timer for rotating "Did You Know" tips ───────────────────
        private readonly DispatcherTimer _tipTimer;
        private int _tipIndex = 0;

        // ── Did You Know tip list (rotates every 15 seconds) ─────────
        private readonly List<string> _didYouKnowTips = new()
        {
            "South Africa ranks among the top targeted countries for cybercrime in Africa.",
            "The Cybercrimes Act (2020) makes hacking and ransomware attacks criminal offences in SA.",
            "81% of data breaches are caused by weak or stolen passwords.",
            "POPIA (2021) gives South Africans the right to know what data companies hold about them.",
            "Phishing attacks account for over 90% of successful cyberattacks globally.",
            "Using 2FA can block up to 99.9% of automated account attacks.",
            "Ransomware attacks increased by 150% in South Africa between 2020 and 2023.",
            "Never plug in a USB drive you found — it could be loaded with malware.",
            "Public Wi-Fi at malls and coffee shops is a hotspot for man-in-the-middle attacks.",
            "A strong password takes over 34,000 years to crack with brute force."
        };

        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public MainWindow()
        {
            InitializeComponent();

            // Initialise memory and engine
            _memory = new MemoryStore();
            _engine = new ResponseEngine(_memory);

            // Play WAV voice greeting on startup
            AudioHelper.PlayGreeting("Audio/welcome.wav");

            // Set up the rotating tip timer (fires every 15 seconds)
            _tipTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };
            _tipTimer.Tick += RotateTip;
            _tipTimer.Start();

            // Focus the name input box when the window loads
            Loaded += (s, e) => NameInputBox.Focus();
        }

        // ============================================================
        // NAME ENTRY SCREEN
        // ============================================================

        /// <summary>
        /// Called when user presses Enter in the name input box.
        /// </summary>
        private void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartChat_Click(sender, e);
        }

        /// <summary>
        /// Called when user clicks "Start Chatting".
        /// Validates the name, stores it in memory, and switches to chat view.
        /// </summary>
        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInputBox.Text.Trim();

            // ── Input Validation ─────────────────────────────────────
            if (string.IsNullOrWhiteSpace(name))
            {
                NameErrorLabel.Text       = "⚠️ Please enter your name to continue.";
                NameErrorLabel.Visibility = Visibility.Visible;
                NameInputBox.Focus();
                return;
            }

            if (name.Length < 2)
            {
                NameErrorLabel.Text       = "⚠️ Name must be at least 2 characters.";
                NameErrorLabel.Visibility = Visibility.Visible;
                NameInputBox.Focus();
                return;
            }

            // ── Valid name — store in memory and start chat ──────────
            _memory.UserName = name;
            _memory.Remember("name", name);

            // Update sidebar labels
            UserNameLabel.Text     = name;
            ChatHeaderLabel.Text   = $"Chat — {name}";

            // Switch panels: hide name entry, show chat
            NameEntryPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility      = Visibility.Visible;

            // Post the bot's opening greeting into the chat
            string greeting =
                $"Welcome, {name}! 🎉 I'm your Cybersecurity Awareness Bot.\n\n" +
                "I'm here to help you stay safe online. You can ask me about phishing, " +
                "passwords, malware, safe browsing, social engineering, SA cyber law, and more.\n\n" +
                "Type 'help' to see all topics, or click one of the quick buttons on the left. " +
                "You can also say 'tell me more' or 'give me another tip' after any response!";

            AddBotMessage(greeting);
            MessageInputBox.Focus();
        }

        // ============================================================
        // CHAT — SENDING MESSAGES
        // ============================================================

        /// <summary>
        /// Called when user presses Enter in the message input box.
        /// </summary>
        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendMessage_Click(sender, e);
        }

        /// <summary>
        /// Called when user clicks Send button.
        /// Reads input, adds user bubble, gets bot response, adds bot bubble.
        /// </summary>
        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string input = MessageInputBox.Text.Trim();

            // ── Input validation ─────────────────────────────────────
            if (string.IsNullOrWhiteSpace(input))
                return; // Silently ignore empty sends

            if (input.Length > 500)
            {
                AddBotMessage("⚠️ Your message is very long. Please keep it shorter and try again.");
                MessageInputBox.Clear();
                return;
            }

            // ── Display the user's message ───────────────────────────
            AddUserMessage(input);
            MessageInputBox.Clear();

            // ── Detect sentiment ─────────────────────────────────────
            Sentiment sentiment = SentimentDetector.Detect(input);
            UpdateSentimentLabel(sentiment);

            // ── Build the ResponseProcessor delegate ─────────────────
            // This delegate wraps the raw response with sentiment and memory prefixes
            ResponseProcessor processor = (rawResponse) =>
            {
                string sentimentPrefix = SentimentDetector.GetSentimentResponse(sentiment);
                string memoryPrefix    = _memory.GetPersonalisedPrefix();

                // Only add memory prefix if it adds value and isn't duplicating sentiment prefix
                string prefix = !string.IsNullOrEmpty(sentimentPrefix)
                    ? sentimentPrefix
                    : memoryPrefix;

                return string.IsNullOrEmpty(prefix)
                    ? rawResponse
                    : prefix + rawResponse;
            };

            // ── Get response from engine, passing the delegate ───────
            string response = _engine.GetResponse(input, processor);

            // ── Handle memory recall — if user mentioned interest ─────
            if (input.ToLower().Contains("interested in") || input.ToLower().Contains("curious about"))
            {
                response += $"\n\n🧠 Got it! I'll remember that, {_memory.UserName}.";
                UpdateMemoryPanel();
            }

            // ── Display bot response ─────────────────────────────────
            AddBotMessage(response);

            // ── Update sidebar panels ────────────────────────────────
            UpdateMemoryPanel();
            UpdateMessageCount();

            // ── Scroll to bottom ─────────────────────────────────────
            ScrollToBottom();
        }

        /// <summary>
        /// Called when a quick topic button is clicked on the left panel.
        /// Simulates the user typing that topic and sends it.
        /// </summary>
        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string topic)
            {
                // If still on name entry screen, ignore quick buttons
                if (ChatPanel.Visibility != Visibility.Visible)
                    return;

                MessageInputBox.Text = topic;
                SendMessage_Click(sender, e);
            }
        }

        // ============================================================
        // MESSAGE BUBBLE BUILDERS
        // ============================================================

        /// <summary>
        /// Adds a user message bubble (right-aligned, cyan background) to the chat.
        /// </summary>
        private void AddUserMessage(string text)
        {
            // Outer container — right-aligned
            Border bubble = new Border
            {
                Background    = new SolidColorBrush(Color.FromRgb(0, 68, 85)),
                CornerRadius  = new CornerRadius(12, 12, 2, 12),
                Padding       = new Thickness(14, 10, 14, 10),
                Margin        = new Thickness(60, 4, 0, 4),
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxWidth      = 480
            };

            StackPanel inner = new StackPanel();

            // "You" label
            inner.Children.Add(new TextBlock
            {
                Text       = $"👤 {_memory.UserName}",
                FontSize   = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                Margin     = new Thickness(0, 0, 0, 4)
            });

            // Message text
            inner.Children.Add(new TextBlock
            {
                Text         = text,
                FontSize     = 13,
                Foreground   = new SolidColorBrush(Color.FromRgb(230, 237, 243)),
                TextWrapping = TextWrapping.Wrap,
                LineHeight   = 20
            });

            bubble.Child = inner;
            ChatMessagesPanel.Children.Add(bubble);
        }

        /// <summary>
        /// Adds a bot message bubble (left-aligned, dark background) to the chat.
        /// </summary>
        private void AddBotMessage(string text)
        {
            Border bubble = new Border
            {
                Background   = new SolidColorBrush(Color.FromRgb(33, 38, 45)),
                CornerRadius = new CornerRadius(12, 12, 12, 2),
                Padding      = new Thickness(14, 10, 14, 10),
                Margin       = new Thickness(0, 4, 60, 4),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth     = 520,
                BorderBrush  = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
                BorderThickness = new Thickness(1)
            };

            StackPanel inner = new StackPanel();

            // "Bot" label
            inner.Children.Add(new TextBlock
            {
                Text       = "🤖 CyberBot",
                FontSize   = 11,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(63, 185, 80)),
                Margin     = new Thickness(0, 0, 0, 4)
            });

            // Response text
            inner.Children.Add(new TextBlock
            {
                Text         = text,
                FontSize     = 13,
                Foreground   = new SolidColorBrush(Color.FromRgb(230, 237, 243)),
                TextWrapping = TextWrapping.Wrap,
                LineHeight   = 22
            });

            bubble.Child = inner;
            ChatMessagesPanel.Children.Add(bubble);

            ScrollToBottom();
        }

        // ============================================================
        // SIDEBAR UPDATE METHODS
        // ============================================================

        /// <summary>Updates the sentiment emoji label in the sidebar.</summary>
        private void UpdateSentimentLabel(Sentiment sentiment)
        {
            SentimentLabel.Text = SentimentDetector.GetSentimentLabel(sentiment);

            // Colour the label based on sentiment
            SentimentLabel.Foreground = sentiment switch
            {
                Sentiment.Worried    => new SolidColorBrush(Color.FromRgb(248, 81, 73)),
                Sentiment.Frustrated => new SolidColorBrush(Color.FromRgb(240, 197, 67)),
                Sentiment.Curious    => new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                Sentiment.Happy      => new SolidColorBrush(Color.FromRgb(63, 185, 80)),
                Sentiment.Angry      => new SolidColorBrush(Color.FromRgb(248, 81, 73)),
                _                    => new SolidColorBrush(Color.FromRgb(139, 148, 158))
            };
        }

        /// <summary>Updates the memory panel in the sidebar with current stored values.</summary>
        private void UpdateMemoryPanel()
        {
            MemoryInterestLabel.Text  = string.IsNullOrEmpty(_memory.Interest)
                ? "Interest: —"
                : $"Interest: {_memory.Interest}";

            MemoryLastTopicLabel.Text = string.IsNullOrEmpty(_memory.LastTopic)
                ? "Last topic: —"
                : $"Last topic: {_memory.LastTopic}";

            MemoryConcernLabel.Text   = string.IsNullOrEmpty(_memory.Concern)
                ? "Concern: —"
                : $"Concern: {_memory.Concern}";
        }

        /// <summary>Updates the message count label in the sidebar.</summary>
        private void UpdateMessageCount()
        {
            MessageCountLabel.Text = $"Messages: {_memory.MessageCount}";
        }

        // ============================================================
        // ROTATING TIP
        // ============================================================

        /// <summary>
        /// Fires every 15 seconds to rotate the "Did You Know" tip in the sidebar.
        /// </summary>
        private void RotateTip(object? sender, EventArgs e)
        {
            _tipIndex = (_tipIndex + 1) % _didYouKnowTips.Count;
            TipLabel.Text = _didYouKnowTips[_tipIndex];
        }

        // ============================================================
        // SCROLL HELPER
        // ============================================================

        /// <summary>Scrolls the chat ScrollViewer to the bottom after each message.</summary>
        private void ScrollToBottom()
        {
            // Dispatch to ensure the UI has rendered the new bubble before scrolling
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                ChatScrollViewer.ScrollToBottom();
            }));
        }
    }
}
