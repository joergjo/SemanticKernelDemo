# Semantic Kernel Pirate Chat Demo

A humorous C# and .NET 8 application showcasing the power of the Semantic Kernel framework through a chat-based pirate game inspired by _Sid Meier's Pirates!_. This demo demonstrates how to build an AI-powered game using Retrieval Augmented Generation (RAG) with Redis as a vector store.

## 🏴‍☠️ Features

- Interactive chat-based pirate adventure game
- AI-powered conversations using Azure OpenAI
- Vector-based ship data retrieval using Redis
- Semantic search for ship information
- Dynamic game encounters and battles

## 🏗️ Architecture

The solution consists of two main projects:

- **DataLoader**: Seeds the Redis vector store with ship descriptions for RAG functionality
- **PirateChat**: The main chat application implementing the game logic and AI interactions

## 🚀 Prerequisites

- .NET 8 SDK
- Docker and Docker Compose
- Azure OpenAI Service account with:
  - A chat completion model deployment (e.g., `gpt-4o-mini`)
  - An embedding model deployment (e.g., `text-embedding-3-small`)

## ⚙️ Setup and Configuration

### 1. Start the Redis Container

First, start the Redis vector store using Docker Compose:

```bash
docker compose up -d
```

This will start a Redis Stack container with:
- Redis server on port `16379`
- Redis Insight web interface on port `8001`

### 2. Configure Azure OpenAI Settings

You need to configure Azure OpenAI settings for both projects. You can use either .NET user secrets or environment variables.

#### Required Settings

**For DataLoader:**
- `Azure:OpenAI:Deployment` - Name of your embedding model deployment (e.g., `text-embedding-3-small`)
- `Azure:OpenAI:Endpoint` - Your Azure OpenAI endpoint URL
- `Azure:OpenAI:Key` - Your Azure OpenAI API key

**For PirateChat:**
- `Azure:OpenAI:Deployment` - Name of your chat completion model deployment (e.g., `gpt-4o-mini`)
- `Azure:OpenAI:EmbeddingDeployment` - Name of your embedding model deployment (e.g., `text-embedding-3-small`)
- `Azure:OpenAI:Endpoint` - Your Azure OpenAI endpoint URL
- `Azure:OpenAI:Key` - Your Azure OpenAI API key

#### Option A: Using .NET User Secrets (Recommended for Development)

For DataLoader:
```bash
cd DataLoader
dotnet user-secrets set "Azure:OpenAI:Deployment" "text-embedding-3-small"
dotnet user-secrets set "Azure:OpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "Azure:OpenAI:Key" "your-api-key"
```

For PirateChat:
```bash
cd PirateChat
dotnet user-secrets set "Azure:OpenAI:Deployment" "gpt-4o-mini"
dotnet user-secrets set "Azure:OpenAI:EmbeddingDeployment" "text-embedding-3-small"
dotnet user-secrets set "Azure:OpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "Azure:OpenAI:Key" "your-api-key"
```

#### Option B: Using Environment Variables

When using environment variables, replace the `:` in configuration keys with `__`:

```bash
export Azure__OpenAI__Deployment="gpt-4o-mini"
export Azure__OpenAI__EmbeddingDeployment="text-embedding-3-small"
export Azure__OpenAI__Endpoint="https://your-resource.openai.azure.com/"
export Azure__OpenAI__Key="your-api-key"
```

### 3. Seed the Vector Store

Run the DataLoader project once to populate the Redis vector store with ship data:

```bash
cd DataLoader
dotnet run
```

This will:
- Connect to the Redis vector store
- Generate embeddings for ship descriptions using your embedding model
- Store the ship data and embeddings in Redis

### 4. Run the Pirate Chat Game

Now you can start the main application:

```bash
cd PirateChat
dotnet run
```

## 🎮 How to Play

Once PirateChat is running, you'll enter an interactive chat session where you can:

- Navigate the high seas as a pirate captain
- Engage in conversations with the AI game master
- Encounter other ships and engage in battles
- Make decisions that affect your pirate adventure
- Ask about different ships (the AI will use RAG to provide detailed information)

## 🔧 Development

### Building the Solution

```bash
dotnet build
```

## 📁 Project Structure

```
SemanticKernelDemo/
├── SemanticKernelDemos.sln
├── compose.yaml                 # Docker Compose for Redis
├── DataLoader/
│   ├── DataLoader.csproj
│   ├── Program.cs              # Vector store seeding logic
│   └── Ship.cs                 # Ship data model
└── PirateChat/
    ├── PirateChat.csproj
    ├── Program.cs              # Main chat application
    ├── RandomEncounters.cs     # Game encounter logic
    ├── Ship.cs                 # Ship data model
    └── plugins/
        └── Fight/              # Semantic Kernel plugins
            ├── config.json
            └── skprompt.txt
```

## 🐛 Troubleshooting

### Redis Connection Issues
- Ensure Docker is running and the Redis container is started
- Check that port `16379` is not blocked by firewall
- Verify the Redis container is healthy: `docker compose ps`

### Azure OpenAI Configuration Issues
- Verify your Azure OpenAI endpoint URL is correct
- Ensure your API key is valid and has not expired
- Check that your model deployments exist and are active
- Confirm you're using the correct deployment names

### Missing Dependencies
If you encounter build errors, restore NuGet packages:
```bash
dotnet restore
```

## 🌊 Set Sail!

Ready to embark on your pirate adventure? Follow the setup steps above, and may the winds be at your back, captain! 🏴‍☠️

---

*This project is a demonstration of Semantic Kernel capabilities and is intended for educational and entertainment purposes.*
