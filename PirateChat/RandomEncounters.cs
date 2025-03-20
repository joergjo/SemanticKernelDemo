using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace PirateChat;

public class RandomEncounters
{
    // private static readonly List<string> _ships =
    // [
    //     "a Spanish galleon",
    //     "a French frigate",
    //     "a pirate sloop",
    //     "a Dutch fluyt",
    //     "an English pinnace",
    //     "an English merchantman",
    // ];

    private readonly Dictionary<int, int> _combatTable = new()
    {
        { 0, 0 },
        { 1, 0 },
        { 2, 1 },
        { 3, 1 },
        { 4, 1 },
        { 5, 1 }
    };

    [KernelFunction]
    [Description("Generate a random ship type")]
    // public string Generate() => _ships[Random.Shared.Next(_ships.Count)];
    public int Generate() => Random.Shared.Next(6);

    // [KernelFunction]
    // [Description("Determine the outcome of a fight")]
    // public int Fight(int shipType)
    // {
    //     return _combatTable.GetValueOrDefault(shipType, 0);
    // }
}