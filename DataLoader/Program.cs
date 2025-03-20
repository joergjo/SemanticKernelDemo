// See https://aka.ms/new-console-template for more information

#pragma warning disable SKEXP0010

using DataLoader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Redis;
using Microsoft.SemanticKernel.Embeddings;
using StackExchange.Redis;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();

// Check our Azure OpenAI configuration
var deployment = configuration["Azure:OpenAI:Deployment"];
var endpoint = configuration["Azure:OpenAI:Endpoint"];
var key = configuration["Azure:OpenAI:Key"];

if (deployment is not { Length: > 0 } || endpoint is not { Length: > 0 } || key is not { Length: > 0 })
{
    Console.WriteLine("Please set the following environment variables:");
    Console.WriteLine("Azure:OpenAI:Deployment");
    Console.WriteLine("Azure:OpenAI:Endpoint");
    Console.WriteLine("Azure:OpenAI:Key");
    Environment.Exit(1);
}

// Create our vector store collection
Console.WriteLine("Connecting to vector store and creating collection...");
var vectorStore = new RedisVectorStore(ConnectionMultiplexer.Connect("localhost:6379").GetDatabase());
var shipData = ShipData.GetShips();
var shipCollection = vectorStore.GetCollection<string, Ship>("pirate-ships");
await shipCollection.CreateCollectionIfNotExistsAsync();

// Generate embeddings for each ship
Console.WriteLine("Generating embeddings...");
var embeddingGenerationService = new AzureOpenAITextEmbeddingGenerationService(deployment, endpoint, key);

foreach (var ship in shipData.Where(ship => ship.Description is { Length: > 0 }))
{
    ship.Vector = await embeddingGenerationService.GenerateEmbeddingAsync(ship.Description!);
    await shipCollection.UpsertAsync(ship);
}

// Test our vectorized search
Console.WriteLine("Running test query...");

// Vectorize our query
const string query = "A heavily armend ship.";
var queryEmbedding = await embeddingGenerationService.GenerateEmbeddingAsync(query);

var searchOptions = new VectorSearchOptions<Ship>
{
    Top = 2,
    VectorProperty = s => s.Vector,
    
};

// Query vector store for similar ships
var results = await shipCollection.VectorizedSearchAsync(queryEmbedding, searchOptions);
await foreach (var result in results.Results)
{
    Console.WriteLine($"Type: {result.Record.ShipType}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}

Console.WriteLine("Done!");