using Microsoft.Extensions.VectorData;

namespace DataLoader;

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

public static class ShipData
{
    public static List<Ship> GetShips() => 
    [
        new() { Id = "0", ShipType = "Galleon", Description = "Galleons are a large ship type predominantly used by the Spanish, boasting the heaviest armor of any ship and a massive complement weaponry. Their maneuverability, however, is considered to be the very worst." },
        new() { Id = "1", ShipType = "Frigate", Description = "The Frigate is one of the larger ship types of dedicated warships, armed with plenty of cannons and a large crew." },
        new() { Id = "2", ShipType = "Sloop", Description = "The Sloop is fast and sleek vessel, with relatively-low firepower, " },
        new() { Id = "3", ShipType = "Fluyt", Description = "The Fluyt is a merchant class ship. It is used exclusively by the Dutch and is rather ungainly." },
        new() { Id = "4", ShipType = "Pinnace", Description = "The small, weak hull on a Pinnace means that it cannot survive prolonged combat with enemies. The Pinnace does not carry enough guns to make any significant impact on enemy ships." },
        new() { Id = "5", ShipType = "Merchantman", Description = "The Merchantman is a solidly-medium-sized trading vessel, reasonably armed and (in some games) averagely maneuverable." },
    ];
}
