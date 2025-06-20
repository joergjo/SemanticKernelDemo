// See https://aka.ms/new-console-template for more information

#pragma warning disable SKEXP0010

using DataLoader;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Redis;
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
var vectorStore = new RedisVectorStore(ConnectionMultiplexer.Connect("localhost:16379").GetDatabase());
var shipCollection = vectorStore.GetCollection<string, Ship>("pirate-ships");
await shipCollection.EnsureCollectionExistsAsync();

var shipData = ShipData.GetShips();

// Build the kernel
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIEmbeddingGenerator(deployment, endpoint, key)
    .Build();
var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

// Generate embeddings for each ship
Console.WriteLine("Generating embeddings...");
var tasks = shipData.Select(entry => Task.Run(async () =>
       {
           entry.DescriptionEmbedding = (await embeddingGenerator.GenerateAsync(entry.Description)).Vector;
       }));
await Task.WhenAll(tasks);

// Upsert the ships into the vector store
await shipCollection.UpsertAsync(shipData);

// Test our vectorized search
Console.WriteLine("Running test query...");

// Vectorize our query
const string query = "A heavily armend ship.";
var queryEmbedding = (await embeddingGenerator.GenerateAsync(query)).Vector;

// Query vector store for similar ships
var results = shipCollection.SearchAsync(queryEmbedding, 2);
await foreach (var result in results)
{
    Console.WriteLine($"Type: {result.Record.ShipType}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}

Console.WriteLine("Done!");