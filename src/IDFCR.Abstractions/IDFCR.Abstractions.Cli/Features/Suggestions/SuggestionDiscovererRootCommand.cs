
using IDFCR.Abstractions.Cli.Extensions;
using IDFCR.Abstractions.Cli.ManagedStreams;
using IDFCR.Abstractions.Cli.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace IDFCR.Abstractions.Cli.Features.Suggestions;

/// <summary>
/// Represents the root command for the suggestion discoverer feature in the CLI application. This command serves as the entry point for all suggestion-related subcommands and operations, allowing users to access and manage suggestions effectively within the CLI environment.
/// </summary>
public class SuggestionDiscovererRootCommand(IServiceProvider services, IManagedStream managedStream)
{
    
    protected async Task<bool> InvokeAsync(IEnumerable<string> command, CancellationToken cancellationToken)
    {
        var parameters = new ArgumentParameters(command);

        if (!parameters.TryGetValue("get-completions", out var parameter))
        {
            return false; 
        }

        if (!parameters.TryGetValue("command-ast", out var commandAst))
        {
            //we have nothing to suggest, returning true so the command parser/CLI routing engine attempts to parse it any further.
            return true;
        }

        int? cursorPosition  = 0;
        if (parameters.TryGetValue("cursor position", out var position) && position.IsFlag && int.TryParse(position.Value, out var _cursorPosition))
        {
            cursorPosition = _cursorPosition;
        }

        //TODO: figure out what commands are being summoned based on the AST and cursor position

        // Helper logic to find the active context
        var fullText = commandAst.Value ?? string.Empty;
        var effectiveCursor = cursorPosition ?? fullText.Length;

        // Slice the text up to the cursor so we don't look at things to the right of it
        var textToAnalyze = fullText.Substring(0, Math.Min(effectiveCursor, fullText.Length));

        // Tokenize (simplistic split, but handles your current 'tags r' case)
        var tokens = textToAnalyze.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // If the text ends with a space, the user is looking for a NEW token
        bool isNewToken = textToAnalyze.EndsWith(" ");
        string partialToken = isNewToken ? "" : (tokens.LastOrDefault() ?? "");

        var rootCommands = services.GetServices<IInjectableCommandOperationRoot>()
            .ToArray();

        var commands = services.GetServices<IInjectableCommandOperation>()
            .ToArray();


        // 1. Determine the "Breadcrumbs"
        // If tokens are ["tags", "read"], and we are starting a new token, 
        // we look for sub-commands or flags of "read".
        var commandPath = isNewToken ? tokens : tokens.Take(tokens.Length - 1).ToArray();

        // 2. Resolve the current scope
        // This is a simplified example of how you might crawl your DI-provided commands
        IEnumerable<string> suggestions = Array.Empty<string>();

        if (commandPath.Length <= 1)
        {
            // Suggesting Root Commands (e.g., 'tags')
            suggestions = rootCommands
                .Select(c => c.Name) // Assuming your interface has a name property
                .Where(name => name.StartsWith(partialToken, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            // Suggesting Sub-commands or Flags
            // You would find the command matching commandPath.Last() 
            // and reflect its properties/methods or child services.
            var activeCommand = commands.FirstOrDefault(c => c.Name.Equals(commandPath.Last(), StringComparison.OrdinalIgnoreCase));

            if (activeCommand != null)
            {
                // Combine sub-commands and flags (e.g., --name-contains)
                suggestions = activeCommand()
                    .Concat(activeCommand.GetAvailableFlags())
                    .Where(s => s.StartsWith(partialToken, StringComparison.OrdinalIgnoreCase));
            }
        }

        // 3. Output
        foreach (var suggestion in suggestions)
        {
            await managedStream.Out.WriteLineAsync(suggestion, cancellationToken);
        }

        await managedStream.Out.WriteLineAsync("<calculated suggestion result here>", cancellationToken);

        return true;
    }
}
