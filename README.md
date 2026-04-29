# 🔐 MyBasicChatbot — Part 2: WPF GUI

[![.NET CI Build](https://github.com/SonnyBoyModise/MyBasicChatbot/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/SonnyBoyModise/MyBasicChatbot/actions/workflows/dotnet-ci.yml)

Part 2 of the Cybersecurity Awareness Bot. Extends Part 1 with a full **WPF GUI**, sentiment detection, memory/recall, random responses, and conversation flow.

---

## New Features in Part 2

| Feature | Description |
|---|---|
| 🖥️ WPF GUI | Dark cybersecurity-themed interface with left sidebar and chat panel |
| 🎭 Sentiment Detection | Detects worried, curious, frustrated, happy, angry moods and adjusts responses |
| 🧠 Memory & Recall | Remembers user's name, interests, concerns, and last topic |
| 🎲 Random Responses | Multiple responses per topic — randomly selected each time |
| 💬 Conversation Flow | "Tell me more", "give me another tip", "explain more" follow-ups |
| ⚡ Quick Topic Buttons | One-click topic shortcuts on the left panel |
| 💡 Rotating Tips | "Did You Know" panel rotates cybersecurity facts every 15 seconds |
| 🔊 Voice Greeting | WAV greeting plays on startup (carried over from Part 1) |

---

## How to Run

```bash
git clone https://github.com/SonnyBoyModise/MyBasicChatbot.git
cd MyBasicChatbot
dotnet run
```

Requires: .NET 8 SDK on Windows.

---

## Project Structure

```
MyBasicChatbot/
├── Audio/welcome.wav
├── Helpers/
│   ├── AudioHelper.cs       # WAV playback
│   ├── MemoryStore.cs       # User memory (Dictionary<string,string>)
│   ├── ResponseEngine.cs    # Keyword + random responses + delegate
│   └── SentimentDetector.cs # Mood detection from user input
├── App.xaml / App.xaml.cs
├── MainWindow.xaml          # WPF layout
├── MainWindow.xaml.cs       # Chat logic, UI updates
└── MyBasicChatbot.csproj
```
