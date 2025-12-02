using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoesOfEtherion.DeveloperConsole.Commands
{
    public class CommandDatabase : Singleton<CommandDatabase>
    {
        private readonly Dictionary<string, IConsoleCommand> commands = new();
        private readonly Dictionary<string, List<string>> commandCategories = new();

        public void RegisterCommand(IConsoleCommand command, string category = "General")
        {
            if (commands.ContainsKey(command.Key.ToLower()))
            {
                ConsoleLogger.Log($"Warning: Command '{command.Key}' is already registered");
                return;
            }

            commands[command.Key.ToLower()] = command;

            if (!commandCategories.ContainsKey(category))
                commandCategories[category] = new List<string>();

            commandCategories[category].Add(command.Key.ToLower());
        }

        public bool ExecuteCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var parts = ParseInput(input);
            if (parts.Length == 0)
                return false;

            string commandKey = parts[0].ToLower();

            // First check if the command exist
            // Avoid checking arguments if command is not found
            if (!commands.ContainsKey(commandKey))
            {
                ConsoleLogger.Log($"Unknown command: '{commandKey}'. Type 'help' for available commands.");
                return false;
            }

            List<Argument> arguments = parts.Skip(1).Select(a => new Argument(a)).ToList();

            return commands[commandKey].Execute(arguments);
        }

        public bool TryGetCommand(string key, out IConsoleCommand command)
        {
            return commands.TryGetValue(key.ToLower(), out command);
        }

        public IEnumerable<IConsoleCommand> GetAllCommands()
        {
            return commands.Values;
        }

        public IEnumerable<IConsoleCommand> GetCommandsByCategory(string category)
        {
            if (!commandCategories.ContainsKey(category))
                return Enumerable.Empty<IConsoleCommand>();

            return commandCategories[category]
                .Select(key => commands[key])
                .Where(c => c != null);
        }

        public Dictionary<string, List<IConsoleCommand>> GetAllCommandsByCategory()
        {
            var result = new Dictionary<string, List<IConsoleCommand>>();

            foreach (var category in commandCategories.Keys)
            {
                result[category] = GetCommandsByCategory(category).ToList();
            }

            return result;
        }

        private string[] ParseInput(string input)
        {
            var result = new List<string>();
            int currentPos = 0;
            bool inQuotes = false;
            string currentToken = "";

            while (currentPos < input.Length)
            {
                char currentChar = input[currentPos];

                if (currentChar == '"')
                {
                    inQuotes = !inQuotes;
                    currentPos++;
                }
                else if (currentChar == ' ' && !inQuotes)
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        result.Add(currentToken);
                        currentToken = "";
                    }
                    currentPos++;
                }
                else
                {
                    currentToken += currentChar;
                    currentPos++;
                }
            }

            if (!string.IsNullOrEmpty(currentToken))
            {
                result.Add(currentToken);
            }

            return result.ToArray();
        }
    }
}