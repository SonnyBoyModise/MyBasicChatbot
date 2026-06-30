# MyBasicChatbot — Part 3 (POE)

[![.NET CI Build](https://github.com/SonnyBoyModise/MyBasicChatbot/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/SonnyBoyModise/MyBasicChatbot/actions/workflows/dotnet-ci.yml)

Final part of the Cybersecurity Awareness Bot. Builds on Parts 1 and 2 with a Task Assistant, Quiz, NLP, and Activity Log.

## New Features

| Feature | Description |
|---|---|
| Task Assistant | Add, view, complete, and delete cybersecurity tasks stored in MySQL |
| Quiz | 12 questions (multiple choice + true/false) with scoring and feedback |
| NLP | Detects intent from natural language — "add task", "start quiz", etc. |
| Activity Log | Type "show activity log" to see a history of what the bot has done |

## MySQL Setup (Required for Task Storage)

1. Install MySQL from https://dev.mysql.com/downloads/
2. Open MySQL and run: `CREATE DATABASE cyberbotdb;`
3. Open `Helpers/DatabaseHelper.cs` and update the connection string with your password

The app works without MySQL — tasks will be stored in memory for the session.

## How to Run

```bash
git clone https://github.com/SonnyBoyModise/MyBasicChatbot.git
cd MyBasicChatbot
dotnet run
```

## Project Structure

```
MyBasicChatbot/
├── Audio/welcome.wav
├── Helpers/
│   ├── ActivityLog.cs       # Records bot actions
│   ├── AudioHelper.cs       # WAV playback
│   ├── DatabaseHelper.cs    # MySQL operations
│   ├── MemoryStore.cs       # User memory
│   ├── NLPProcessor.cs      # Intent detection
│   ├── QuizEngine.cs        # 12 quiz questions + scoring
│   ├── ResponseEngine.cs    # Keyword + random responses
│   ├── SentimentDetector.cs # Mood detection
│   └── TaskItem.cs          # Task model
├── App.xaml / App.xaml.cs
├── MainWindow.xaml          # Full GUI layout
├── MainWindow.xaml.cs       # All logic
└── MyBasicChatbot.csproj
```
