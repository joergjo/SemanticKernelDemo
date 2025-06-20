using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;

namespace PirateChat;

public class Ship
{
    [VectorStoreKey]
    public required string Id { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public string? ShipType { get; set; }
    
    [VectorStoreData(IsFullTextIndexed = true)]
    public string? Description { get; set; }
    
    [VectorStoreVector(1536)]
    public ReadOnlyMemory<float> DescriptionEmbedding { get; set; }
}

public class ShipSearch(
    VectorStoreCollection<string, Ship> collection, 
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
{
    [KernelFunction]
    [Description("Describe a ship based on a query.")]
    public async Task<string> SearchAsync(string query)
    {
        var queryEmbedding = (await embeddingGenerator.GenerateAsync(query)).Vector;

        var results = collection.SearchAsync(queryEmbedding, top: 1);
        var completion = string.Empty;
        await foreach (var result in results)
        {
            completion = result.Record.Description!;
            break;
        }

        return completion;
    }
}
