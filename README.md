<div align="center">
    <a href="https://github.com/Ch1py7/twitch?tab=readme-ov-file#-project-structure">Project Structure</a>
    <span>&nbsp;❖&nbsp;</span>
    <a href="https://github.com/Ch1py7/twitch?tab=readme-ov-file#-built-with">Built With</a>
    <span>&nbsp;❖&nbsp;</span>
    <a href="https://github.com/Ch1py7/twitch?tab=readme-ov-file#-contributing">Contributing</a>
</div>

# 🤖 Twitch Bot
**Twitch Bot** is a backend application designed to efficiently retrieve and process chat messages from Twitch.

This repository contains **only** the backend implementation. If you require a frontend interface, you will need to develop one that communicates via WebSockets.

## 🎯 Features
- 🔍 **Easy Integration** – Simply set the required environment variables, and the bot is ready to run.
- 🚀 **Enhanced Readability** – Messages are parsed and formatted for better visualization.

## 🛠 Installation

### 1️⃣ **Set Up Your Environment**  
To authenticate with Twitch, you must set the following system/user environment variables:
- `ClientId`
- `Secret`

#### How to Set Environment Variables:
1. Press the **Super Key** (Windows Key) and type `env`.
2. Select **Edit the system environment variables**.
3. Click **Environment Variables** and add the required values.

For more details on obtaining these credentials, refer to the official [Twitch Documentation](https://dev.twitch.tv/docs/authentication/register-app/).

### 2️⃣ **Clone and Install Dependencies**
```bash
# Clone this repository
git clone https://github.com/Ch1py7/twitch

# Navigate into the project directory
cd twitch

# Install dependencies
dotnet restore
```

### 3️⃣ **Create Token File**
- Navigate to `D:\` and create a file named `token.json`. It can be empty or contain the following structure:
```json
{
  "AccessToken": "your_access_token",
  "RefreshToken": "your_refresh_token",
  "ExpiresIn": 0,
  "ObtainedAt": 0
}
```

### 4️⃣ **Start the Project**
- Open a terminal in the project root directory and run:
```bash
dotnet run
```

## 🏗️ Project Structure
```plaintext
📦 api
 ┃ 📂 Application
 ┃ ┣ 📂 Services
 ┃ ┣ 📂 Repositories
 ┃ ┗ 📜 Config.cs
 ┃ 📂 Domain
 ┃ ┣ 📂 Entities
 ┃ 📂 Infrastructure
 ┃ ┣ 📂 Twitch
 ┃ ┃ ┣ 📂 Irc
 ┃ ┃ ┣ 📂 Parser
 ┃ 📂 Presentation
 ┃ ┗ 📂 Controllers
 ┗ 📜 Program.cs
 ┗ 📜 appsettings.json
 ┗ 📜 .editorconfig
 ┗ 🔑 LICENCE
```

## 🛠 Built With
- **[C#](https://learn.microsoft.com/en-us/dotnet/csharp/)** – A modern, object-oriented programming language.
- **[ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet)** – A framework for building web applications and APIs.
- **[WebSockets](https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API)** – A protocol for real-time communication between clients and servers.

## 🤝 Contributing
Contributions are welcome! Feel free to open an issue or submit a pull request.

## 📄 License
This project is licensed under the [MIT License](https://github.com/Ch1py7/twitch/blob/main/LICENSE).