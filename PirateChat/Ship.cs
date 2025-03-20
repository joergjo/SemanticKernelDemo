using System.ComponentModel;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
#pragma warning disable SKEXP0001

namespace PirateChat;

public class Ship
{
    [VectorStoreRecordKey]
    public required string Id { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public string? ShipType { get; set; }
    
    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public string? Description { get; set; }
    
    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> Vector { get; set; }
}

public class ShipSearch(
    IVectorStoreRecordCollection<string, Ship> collection, 
    ITextEmbeddingGenerationService embeddingGenerationService)
{
    [KernelFunction]
    [Description("Describe a ship based on a query.")]
    public async Task<string> SearchAsync(string query)
    {
        var queryEmbedding = await embeddingGenerationService.GenerateEmbeddingAsync(query);
        var searchOptions = new VectorSearchOptions<Ship>
        {
            Top = 1,
            VectorProperty = s => s.Vector,
        };

        var results = await collection.VectorizedSearchAsync(queryEmbedding, searchOptions);
        var completion = string.Empty;
        await foreach (var result in results.Results)
        {
            completion = result.Record.Description!;
            break;
        }

        return completion;
    }
}
