// MainWindow.xaml.cs — code-behind for the full Part 3 chatbot GUI

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
        // Core helpers
        private readonly MemoryStore    _memory;
        private readonly ResponseEngine _engine;
        private readonly DatabaseHelper _db;
        private readonly QuizEngine     _quiz;
        private readonly ActivityLog    _log;

        // Tracks whether we are waiting for quiz feedback before moving on
        private bool _waitingForNext = false;

        // Rotating tips timer
        private readonly DispatcherTimer _tipTimer;
        private int _tipIndex = 0;

        private readonly List<string> _tips = new()
        {
            "South Africa ranks among the top targeted countries for cybercrime in Africa.",
            "The Cybercrimes Act (2020) makes hacking and ransomware criminal offences in SA.",
            "81% of data breaches are caused by weak or stolen passwords.",
            "POPIA (2021) gives South Africans the right to know what data companies hold about them.",
            "Phishing attacks account for over 90% of successful cyberattacks globally.",
            "Using 2FA can block up to 99.9% of automated account attacks.",
            "Never plug in a USB drive you found — it could be loaded with malware.",
            "Public Wi-Fi is a hotspot for man-in-the-middle attacks. Use mobile data for banking.",
            "A strong 12-character password takes thousands of years to crack with brute force.",
            "Ransomware attacks in South Africa increased significantly between 2020 and 2023."
        };

        // ── Constructor ──────────────────────────────────────────────

        public MainWindow()
        {
            InitializeComponent();

            _memory = new MemoryStore();
            _engine = new ResponseEngine(_memory);
            _db     = new DatabaseHelper();
            _quiz   = new QuizEngine();
            _log    = new ActivityLog();

            // Play voice greeting
            AudioHelper.PlayGreeting("Audio/welcome.wav");

            // Show database connection status in sidebar
            DbStatusLabel.Text      = _db.IsConnected
                ? "Connected to MySQL"
                : "Using memory storage (MySQL not found)";
            DbStatusLabel.Foreground = _db.IsConnected
                ? new SolidColorBrush(Color.FromRgb(63, 185, 80))
                : new SolidColorBrush(Color.FromRgb(240, 197, 67));

            // Rotate tips every 15 seconds
            _tipTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(15) };
            _tipTimer.Tick += (s, e) =>
            {
                _tipIndex = (_tipIndex + 1) % _tips.Count;
                TipLabel.Text = _tips[_tipIndex];
            };
            _tipTimer.Start();

            Loaded += (s, e) => NameInputBox.Focus();
        }

        // ── Name Entry ───────────────────────────────────────────────

        private void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) StartChat_Click(sender, e);
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInputBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                NameErrorLabel.Text       = "Please enter a valid name (at least 2 characters).";
                NameErrorLabel.Visibility = Visibility.Visible;
                return;
            }

            _memory.UserName = name;
            _memory.Remember("name", name);

            UserNameLabel.Text    = name;
            ChatHeaderLabel.Text  = $"Logged in as {name}";

            NameEntryPanel.Visibility  = Visibility.Collapsed;
            MainContentArea.Visibility = Visibility.Visible;

            _log.Add($"Session started for user '{name}'");

            // Welcome message in chat
            AddBotMessage(
                $"Welcome, {name}! I am your Cybersecurity Awareness Bot.\n\n" +
                "Chat: Ask me about phishing, passwords, malware, safe browsing, and more.\n" +
                "Tasks: Use the Tasks tab to manage your cybersecurity to-do list.\n" +
                "Quiz:  Test your knowledge in the Quiz tab.\n\n" +
                "Type 'help' to see all chat topics, or 'show activity log' to see what I have done.");

            MessageInputBox.Focus();
        }

        // ── Navigation ───────────────────────────────────────────────

        private void NavChat_Click(object sender, RoutedEventArgs e)  => ShowPanel("chat");
        private void NavTasks_Click(object sender, RoutedEventArgs e) => ShowPanel("tasks");
        private void NavQuiz_Click(object sender, RoutedEventArgs e)  => ShowPanel("quiz");

        // Hides all panels and shows the requested one
        private void ShowPanel(string panel)
        {
            ChatPanel.Visibility  = Visibility.Collapsed;
            TasksPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility  = Visibility.Collapsed;

            // Reset nav button styles
            BtnNavChat.Style  = (Style)FindResource("NavBtn");
            BtnNavTasks.Style = (Style)FindResource("NavBtn");
            BtnNavQuiz.Style  = (Style)FindResource("NavBtn");

            switch (panel)
            {
                case "chat":
                    ChatPanel.Visibility = Visibility.Visible;
                    BtnNavChat.Style     = (Style)FindResource("NavBtnActive");
                    MessageInputBox.Focus();
                    break;

                case "tasks":
                    TasksPanel.Visibility = Visibility.Visible;
                    BtnNavTasks.Style     = (Style)FindResource("NavBtnActive");
                    LoadTaskList();
                    TaskTitleBox.Focus();
                    break;

                case "quiz":
                    QuizPanel.Visibility = Visibility.Visible;
                    BtnNavQuiz.Style     = (Style)FindResource("NavBtnActive");
                    break;
            }
        }

        // ── Chat ─────────────────────────────────────────────────────

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage_Click(sender, e);
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string input = MessageInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AddUserMessage(input);
            MessageInputBox.Clear();

            // Detect intent using NLP processor
            UserIntent intent = NLPProcessor.DetectIntent(input);

            // Handle special intents
            switch (intent)
            {
                case UserIntent.AddTask:
                    HandleAddTaskFromChat(input);
                    return;

                case UserIntent.ViewTasks:
                    AddBotMessage("Switching to your Tasks panel now!");
                    _log.Add("User asked to view tasks via chat — navigated to Tasks panel");
                    ShowPanel("tasks");
                    return;

                case UserIntent.StartQuiz:
                    AddBotMessage("Let us test your knowledge! Switching to the Quiz now.");
                    _log.Add("User started quiz via chat command");
                    ShowPanel("quiz");
                    return;

                case UserIntent.ShowActivityLog:
                    AddBotMessage(_log.GetLog());
                    return;
            }

            // Normal chat response
            Sentiment sentiment = SentimentDetector.Detect(input);
            UpdateSentimentLabel(sentiment);

            // Use the ResponseProcessor delegate to add sentiment/memory prefix
            ResponseProcessor processor = (raw) =>
            {
                string prefix = SentimentDetector.GetSentimentResponse(sentiment);
                if (string.IsNullOrEmpty(prefix))
                    prefix = _memory.GetPersonalisedPrefix();
                return string.IsNullOrEmpty(prefix) ? raw : prefix + raw;
            };

            string response = _engine.GetResponse(input, processor);
            AddBotMessage(response);

            // Store interest in memory
            if (input.ToLower().Contains("interested in") || input.ToLower().Contains("curious about"))
                AddBotMessage($"I will remember that, {_memory.UserName}.");

            UpdateSidebar();
            ScrollToBottom();
        }

        // Handles "add task Enable 2FA" typed in the chat
        private void HandleAddTaskFromChat(string input)
        {
            string title = NLPProcessor.ExtractTaskTitle(input);
            if (string.IsNullOrWhiteSpace(title))
            {
                AddBotMessage("I'd like to add a task for you! What should the task be called?");
                return;
            }

            // Check if a reminder was mentioned
            int? days     = NLPProcessor.ExtractDays(input);
            DateTime? rem = days.HasValue ? DateTime.Now.AddDays(days.Value) : (DateTime?)null;

            var task = new TaskItem
            {
                Title       = char.ToUpper(title[0]) + title.Substring(1),
                Description = "Added via chat",
                ReminderDate = rem
            };

            _db.AddTask(task);
            _log.Add($"Task added: '{task.Title}'" + (rem.HasValue ? $" (reminder: {rem.Value:dd MMM yyyy})" : ""));

            string reply = $"Task added: '{task.Title}'.\n";
            if (rem.HasValue)
                reply += $"Reminder set for {rem.Value:dd MMM yyyy}.";
            else
                reply += "Would you like to switch to the Tasks tab to add a reminder?";

            AddBotMessage(reply);
            UpdateSidebar();
        }

        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string topic)
            {
                if (MainContentArea.Visibility != Visibility.Visible) return;
                ShowPanel("chat");
                MessageInputBox.Text = topic;
                SendMessage_Click(sender, e);
            }
        }

        // ── Task Management ──────────────────────────────────────────

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            // Hide previous messages
            TaskErrorLabel.Visibility   = Visibility.Collapsed;
            TaskSuccessLabel.Visibility = Visibility.Collapsed;

            string title = TaskTitleBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                TaskErrorLabel.Text       = "Please enter a task title.";
                TaskErrorLabel.Visibility = Visibility.Visible;
                return;
            }

            var task = new TaskItem
            {
                Title        = title,
                Description  = TaskDescBox.Text.Trim(),
                ReminderDate = TaskDatePicker.SelectedDate
            };

            _db.AddTask(task);
            _log.Add($"Task added: '{task.Title}'" +
                     (task.ReminderDate.HasValue ? $" with reminder on {task.ReminderDate.Value:dd MMM yyyy}" : ""));

            // Clear inputs
            TaskTitleBox.Clear();
            TaskDescBox.Clear();
            TaskDatePicker.SelectedDate = null;

            TaskSuccessLabel.Text       = $"Task '{task.Title}' added successfully!";
            TaskSuccessLabel.Visibility = Visibility.Visible;

            LoadTaskList();
        }

        // Loads all tasks from the database and builds task cards in the UI
        private void LoadTaskList()
        {
            TaskListPanel.Children.Clear();
            var tasks = _db.GetAllTasks();

            if (tasks.Count == 0)
            {
                TaskListPanel.Children.Add(new TextBlock
                {
                    Text       = "No tasks yet. Add a cybersecurity task above to get started!",
                    Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                    FontSize   = 13,
                    Margin     = new Thickness(4, 12, 0, 0)
                });
                return;
            }

            foreach (var task in tasks)
            {
                // Build each task as a card
                var card = new Border
                {
                    Background      = new SolidColorBrush(task.IsCompleted
                        ? Color.FromRgb(22, 32, 50)
                        : Color.FromRgb(33, 38, 45)),
                    CornerRadius    = new CornerRadius(8),
                    BorderBrush     = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
                    BorderThickness = new Thickness(1),
                    Padding         = new Thickness(14, 12, 14, 12),
                    Margin          = new Thickness(0, 0, 0, 8)
                };

                var row = new Grid();
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Left: task info
                var info = new StackPanel();

                var titleText = new TextBlock
                {
                    Text       = task.Title,
                    FontSize   = 14,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(task.IsCompleted
                        ? Color.FromRgb(139, 148, 158)
                        : Color.FromRgb(230, 237, 243)),
                    TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : null
                };
                info.Children.Add(titleText);

                if (!string.IsNullOrEmpty(task.Description))
                {
                    info.Children.Add(new TextBlock
                    {
                        Text       = task.Description,
                        FontSize   = 12,
                        Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                        Margin     = new Thickness(0, 3, 0, 0)
                    });
                }

                // Row with reminder + status
                var metaRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
                metaRow.Children.Add(new TextBlock
                {
                    Text       = task.ReminderText,
                    FontSize   = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                    Margin     = new Thickness(0, 0, 14, 0)
                });
                metaRow.Children.Add(new TextBlock
                {
                    Text       = task.StatusText,
                    FontSize   = 11,
                    Foreground = new SolidColorBrush(task.IsCompleted
                        ? Color.FromRgb(63, 185, 80)
                        : Color.FromRgb(240, 197, 67))
                });
                info.Children.Add(metaRow);

                Grid.SetColumn(info, 0);
                row.Children.Add(info);

                // Right: action buttons
                var buttons = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };

                if (!task.IsCompleted)
                {
                    int taskId     = task.Id;
                    string taskTitle = task.Title;

                    var completeBtn = new Button
                    {
                        Content = "✓ Done",
                        Style   = (Style)FindResource("GreenBtn"),
                        Margin  = new Thickness(0, 0, 6, 0)
                    };
                    completeBtn.Click += (s, e) =>
                    {
                        _db.CompleteTask(taskId);
                        _log.Add($"Task completed: '{taskTitle}'");
                        LoadTaskList();
                    };
                    buttons.Children.Add(completeBtn);
                }

                int delId       = task.Id;
                string delTitle = task.Title;
                var deleteBtn   = new Button { Content = "🗑 Delete", Style = (Style)FindResource("RedBtn") };
                deleteBtn.Click += (s, e) =>
                {
                    _db.DeleteTask(delId);
                    _log.Add($"Task deleted: '{delTitle}'");
                    LoadTaskList();
                };
                buttons.Children.Add(deleteBtn);

                Grid.SetColumn(buttons, 1);
                row.Children.Add(buttons);

                card.Child = row;
                TaskListPanel.Children.Add(card);
            }
        }

        // ── Quiz ─────────────────────────────────────────────────────

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quiz.Reset();
            _log.Add("Quiz started");
            QuizStartPanel.Visibility    = Visibility.Collapsed;
            QuizResultPanel.Visibility   = Visibility.Collapsed;
            QuizQuestionPanel.Visibility = Visibility.Visible;
            FeedbackBar.Visibility       = Visibility.Collapsed;
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            if (_quiz.IsFinished)
            {
                ShowQuizResult();
                return;
            }

            _waitingForNext = false;
            FeedbackBar.Visibility = Visibility.Collapsed;

            var q = _quiz.GetCurrentQuestion();

            QuizProgressLabel.Text = $"Question {_quiz.CurrentNumber} of {_quiz.TotalQuestions}";
            QuizScoreLabel.Text    = $"Score: {_quiz.Score}";
            QuizQuestionText.Text  = q.Text;

            // Build answer buttons dynamically
            AnswerButtonsPanel.Children.Clear();
            for (int i = 0; i < q.Options.Count; i++)
            {
                int index = i; // capture for lambda
                var btn = new Button
                {
                    Content = q.Options[i],
                    Style   = (Style)FindResource("AnswerBtn")
                };
                btn.Click += (s, e) => AnswerSelected(index);
                AnswerButtonsPanel.Children.Add(btn);
            }
        }

        private void AnswerSelected(int selectedIndex)
        {
            if (_waitingForNext) return; // prevent double-clicking
            _waitingForNext = true;

            bool correct = _quiz.SubmitAnswer(selectedIndex);
            var  q       = _quiz.GetCurrentQuestion(); // still the same question before MoveNext

            // Actually SubmitAnswer already increments — get previous question for explanation
            // We need to get the explanation from the question that was just answered
            // QuizEngine.SubmitAnswer increments _currentIndex, so we look at index - 1
            // Let's re-get it properly — QuizEngine exposes the explanation via the answered q
            var answeredQ = _quiz.GetPreviousQuestion();

            // Colour the selected button green or red
            if (AnswerButtonsPanel.Children.Count > selectedIndex)
            {
                var btn = (Button)AnswerButtonsPanel.Children[selectedIndex];
                btn.Background = correct
                    ? new SolidColorBrush(Color.FromRgb(22, 50, 22))
                    : new SolidColorBrush(Color.FromRgb(50, 22, 22));
                btn.BorderBrush = correct
                    ? new SolidColorBrush(Color.FromRgb(63, 185, 80))
                    : new SolidColorBrush(Color.FromRgb(248, 81, 73));
            }

            // Show feedback
            string icon = correct ? "✅ Correct!" : "❌ Incorrect.";
            FeedbackText.Text      = $"{icon}  {answeredQ?.Explanation ?? ""}";
            FeedbackText.Foreground = correct
                ? new SolidColorBrush(Color.FromRgb(63, 185, 80))
                : new SolidColorBrush(Color.FromRgb(248, 81, 73));

            FeedbackBar.Visibility = Visibility.Visible;
            QuizScoreLabel.Text    = $"Score: {_quiz.Score}";

            _log.Add($"Quiz: answered Q{_quiz.CurrentNumber - 1} — {(correct ? "correct" : "incorrect")}");

            // Disable all answer buttons after answering
            foreach (Button btn in AnswerButtonsPanel.Children)
                btn.IsEnabled = false;

            // If last question, change button text
            if (_quiz.IsFinished)
                FeedbackBar.Visibility = Visibility.Collapsed; // result screen will show
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (_quiz.IsFinished)
                ShowQuizResult();
            else
                ShowQuestion();
        }

        private void ShowQuizResult()
        {
            QuizQuestionPanel.Visibility = Visibility.Collapsed;
            FeedbackBar.Visibility       = Visibility.Collapsed;
            QuizResultPanel.Visibility   = Visibility.Visible;

            string msg   = _quiz.GetFinalMessage();
            double pct   = (double)_quiz.Score / _quiz.TotalQuestions * 100;

            QuizResultEmoji.Text   = pct >= 70 ? "🏆" : pct >= 50 ? "👍" : "📚";
            QuizResultMessage.Text = msg;
            QuizFinalScore.Text    = $"Final score: {_quiz.Score} out of {_quiz.TotalQuestions}";

            _log.Add($"Quiz completed — final score: {_quiz.Score}/{_quiz.TotalQuestions}");
        }

        private void RestartQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizResultPanel.Visibility = Visibility.Collapsed;
            StartQuiz_Click(sender, e);
        }

        // ── Message Bubbles ──────────────────────────────────────────

        private void AddUserMessage(string text)
        {
            var bubble = new Border
            {
                Background          = new SolidColorBrush(Color.FromRgb(0, 68, 85)),
                CornerRadius        = new CornerRadius(12, 12, 2, 12),
                Padding             = new Thickness(14, 10, 14, 10),
                Margin              = new Thickness(60, 4, 0, 4),
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxWidth            = 480
            };

            var inner = new StackPanel();
            inner.Children.Add(new TextBlock { Text = $"👤 {_memory.UserName}", FontSize = 11, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255)), Margin = new Thickness(0, 0, 0, 4) });
            inner.Children.Add(new TextBlock { Text = text, FontSize = 13, Foreground = new SolidColorBrush(Color.FromRgb(230, 237, 243)), TextWrapping = TextWrapping.Wrap, LineHeight = 20 });

            bubble.Child = inner;
            ChatMessagesPanel.Children.Add(bubble);
        }

        private void AddBotMessage(string text)
        {
            var bubble = new Border
            {
                Background          = new SolidColorBrush(Color.FromRgb(33, 38, 45)),
                CornerRadius        = new CornerRadius(12, 12, 12, 2),
                Padding             = new Thickness(14, 10, 14, 10),
                Margin              = new Thickness(0, 4, 60, 4),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth            = 520,
                BorderBrush         = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
                BorderThickness     = new Thickness(1)
            };

            var inner = new StackPanel();
            inner.Children.Add(new TextBlock { Text = "🤖 CyberBot", FontSize = 11, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(63, 185, 80)), Margin = new Thickness(0, 0, 0, 4) });
            inner.Children.Add(new TextBlock { Text = text, FontSize = 13, Foreground = new SolidColorBrush(Color.FromRgb(230, 237, 243)), TextWrapping = TextWrapping.Wrap, LineHeight = 22 });

            bubble.Child = inner;
            ChatMessagesPanel.Children.Add(bubble);
            ScrollToBottom();
        }

        // ── Sidebar Updates ──────────────────────────────────────────

        private void UpdateSentimentLabel(Sentiment s)
        {
            SentimentLabel.Text = SentimentDetector.GetSentimentLabel(s);
            SentimentLabel.Foreground = s switch
            {
                Sentiment.Worried    => new SolidColorBrush(Color.FromRgb(248, 81, 73)),
                Sentiment.Frustrated => new SolidColorBrush(Color.FromRgb(240, 197, 67)),
                Sentiment.Curious    => new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                Sentiment.Happy      => new SolidColorBrush(Color.FromRgb(63, 185, 80)),
                Sentiment.Angry      => new SolidColorBrush(Color.FromRgb(248, 81, 73)),
                _                    => new SolidColorBrush(Color.FromRgb(139, 148, 158))
            };
        }

        private void UpdateSidebar()
        {
            MemoryInterestLabel.Text  = string.IsNullOrEmpty(_memory.Interest)  ? "Interest: —" : $"Interest: {_memory.Interest}";
            MemoryLastTopicLabel.Text = string.IsNullOrEmpty(_memory.LastTopic) ? "Last topic: —" : $"Last topic: {_memory.LastTopic}";
            MemoryConcernLabel.Text   = string.IsNullOrEmpty(_memory.Concern)   ? "Concern: —" : $"Concern: {_memory.Concern}";
            MessageCountLabel.Text    = $"Messages: {_memory.MessageCount}";
        }

        private void ScrollToBottom()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                ChatScrollViewer.ScrollToBottom()));
        }
    }
}
