# 🔐 MyBasicChatbot — Cybersecurity Awareness Assistant

[![.NET CI Build](https://github.com/SonnyBoyModise/MyBasicChatbot/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/SonnyBoyModise/MyBasicChatbot/actions/workflows/dotnet-ci.yml)

A command-line cybersecurity awareness chatbot built in **C# (.NET 8)** for South African citizens. Developed as Part 1 of the Cybersecurity Awareness Bot project for the Department of Cybersecurity campaign.

---

## Features

- 🔊 **Voice greeting** — plays a recorded WAV welcome message on startup
- 🎨 **ASCII art logo** — cybersecurity-themed header displayed at launch
- 👤 **Personalised interaction** — asks for your name and uses it throughout
- 💬 **Keyword-based responses** — covers phishing, passwords, malware, safe browsing, SA cyber law, and more
- ✅ **Input validation** — handles empty input and unknown queries gracefully
- 🖥️ **Enhanced console UI** — coloured text, typing effect, section headers, dividers

---

## Topics Covered

| Topic | Keywords to try |
|---|---|
| Phishing scams | `phishing`, `phishing email` |
| Password safety | `password`, `password safety` |
| Two-Factor Auth | `two factor` |
| Safe browsing | `safe browsing`, `https` |
| Malware & viruses | `malware`, `ransomware`, `virus` |
| Social engineering | `social engineering` |
| SA cyber law | `south africa`, `cyber law`, `popia` |
| SARS scams | `sars scam` |
| General scams | `scam` |

---

## How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (Windows)
- Windows OS (required for `System.Media.SoundPlayer`)

### Steps
```bash
git clone https://github.com/SonnyBoyModise/MyBasicChatbot.git
cd MyBasicChatbot
dotnet run
```

---

## Project Structure

```
MyBasicChatbot/
├── Audio/
│   └── welcome.wav              # Voice greeting (WAV)
├── Helpers/
│   ├── AudioHelper.cs           # WAV playback via SoundPlayer
│   ├── ChatBot.cs               # Core chatbot logic & conversation loop
│   ├── DisplayHelper.cs         # Console UI: colours, ASCII, typing effect
│   └── ResponseEngine.cs        # Keyword-to-response dictionary
├── .github/
│   └── workflows/
│       └── dotnet-ci.yml        # GitHub Actions CI workflow
├── Program.cs                   # Entry point
├── MyBasicChatbot.csproj        # Project config (net8.0-windows)
└── README.md
```

---

## CI/CD

This project uses **GitHub Actions** for Continuous Integration. On every push to `main`, the workflow:
1. Checks out the code
2. Sets up .NET 8 on a Windows runner
3. Restores dependencies
4. Builds the project in Release mode

---

*Part 1 of the Cybersecurity Awareness Bot project — PROG2A*
