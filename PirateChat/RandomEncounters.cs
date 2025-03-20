using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace PirateChat;

public class RandomEncounters
{
    [KernelFunction]
    [Description("Generate a random ship type")]
    public int Generate() => Random.Shared.Next(6);
}