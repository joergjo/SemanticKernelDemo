// See https://aka.ms/new-console-template for more information

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0070

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;
using OllamaSharp;
using PirateChat;
using StackExchange.Redis;

// Create a configuration stack
var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

// Check our Azure OpenAI configuration
var deployment = configuration["Azure:OpenAI:Deployment"];
var embeddingDeployment = configuration["Azure:OpenAI:EmbeddingDeployment"];
var endpoint = configuration["Azure:OpenAI:Endpoint"];
var key = configuration["Azure:OpenAI:Key"];

if (deployment is not { Length: > 0 } || embeddingDeployment is not { Length: > 0 } ||
    endpoint is not { Length: > 0 } || key is not { Length: > 0 })
{
    Console.WriteLine("Please set the following environment variables:");
    Console.WriteLine("Azure:OpenAI:Deployment");
    Console.WriteLine("Azure:OpenAI:EmbeddingDeployment");
    Console.WriteLine("Azure:OpenAI:Endpoint");
    Console.WriteLine("Azure:OpenAI:Key");
    Environment.Exit(1);
}

var vectorStore = new RedisVectorStore(ConnectionMultiplexer.Connect("localhost:16379").GetDatabase());
var collection = vectorStore.GetCollection<string, Ship>("pirate-ships");

// Create a kernel builder
var builder = Kernel.CreateBuilder();
// Add the Azure OpenAI chat completion service
builder.AddAzureOpenAIChatCompletion(deployment, endpoint, key);
builder.AddAzureOpenAITextEmbeddingGeneration(embeddingDeployment, endpoint, key);

// builder.AddOllamaChatCompletion(modelId: "llama3.2", endpoint: new Uri("http://localhost:11434"));

// Add logging to check token usage etc.
builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));


// Add the RandomEncouters plugin
builder.Plugins.AddFromType<RandomEncounters>();

// Add the Fight plugin
var pluginDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
builder.Plugins.AddFromPromptDirectory(pluginDirectory);

// Create the kernel
var kernel = builder.Build();

var embeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
kernel.Plugins.Add(KernelPluginFactory.CreateFromObject(
    new ShipSearch(collection, embeddingGenerationService)));

// var yaml = File.ReadAllText("fight.yaml");
// var fightFunction = kernel.CreateFunctionFromPromptYaml(yaml);
// var fightPlugin = KernelPluginFactory.CreateFromFunctions("Fight", [fightFunction]);
// kernel.Plugins.Add(fightPlugin);

// Look up the chat completion service
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    Temperature = 1.2,
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};
// var executionSettings = new OllamaPromptExecutionSettings
// {
//     Temperature = 1.2f,
//     FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
// };

// Create a chat history including a system prompt
var history = new ChatHistory(
    """
    From now on, you will talk like a pirate! Please respond to every message in pirate speech.
    Whenever the conversation explicitly says that a ship has been sighted, generate a random ship type 
    using the "Generate" function provided to you. The Generate function will return numbers between 0 and 5.
    These map to the following ship types:
    0: a Spanish galleon
    1: a French frigate
    2: a pirate sloop
    3: a Dutch fluyt
    4: an English pinnace
    5: an English merchantman
    Use the actual name of the ship in your response.

    When the conversation implies that a fight has occurred, use the "Fight" function provided to you,
    using the ship type as parameter and respond. The Fight function will return 0 (defeat) or 1 (victory).

    When being asked to describe one of the ship types above, use the "Describe" function provided to you and respond.
    Do not use any other information you may have on these ships.
    """);

do
{
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("User > ");
    var userInput = Console.ReadLine();
    Console.ResetColor();
    if (userInput is null)
    {
        break;
    }

    history.AddUserMessage(userInput);
    var result =
        await chatCompletionService.GetChatMessageContentAsync(history, executionSettings: executionSettings,
            kernel: kernel);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Assistant > {0}", result);
    Console.ResetColor();
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (true);