This is a C# and .NET 8 based repository that showcases a humorous use case of the Semantic Kernel framework.
It implememnts a chat application that allows the user to build a simple game inspired by _Sid Meier's Pirates!_.
Note that this project does not implement any tests.

## Code Standards
Follow the .NET Framework coding standards and conventions as outlined in the [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
Make sure to use Allman style braces and maintain consistent indentation of 4 spaces. Private fields and private static fields should be prefixed with an underscore (`_`). Private constants are treated
as any other constant and should be named in PascalCase without a prefix.

### Development Flow
- Build: `dotnet build`

## Repository Structure
The repository provides .NET solution `SemanticKernelDemos.sln` with two projects:
- `DataLoader`: This loads ship descriptions used for Retrievasl Augmented Generation (RAG) into a Redis vector store.
- `PirateChat`: This is the main application that implements the chat interface and game logic.

## Key Guidelines
1. Follow C# best practices and idiomatic patterns
2. Maintain existing code structure and organization
3. This a fun demo that does not require unit tests.
