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
            List<string> commandStrings = SplitCommands(input);

            bool allExecuted = true;

            foreach (string cmd in commandStrings)
            {
                string[] parts = ParseArguments(cmd);
                if (parts.Length == 0)
                    continue;

                string commandKey = parts[0].ToLower();

                if (!commands.ContainsKey(commandKey))
                {
                    ConsoleLogger.Log($"Unknown command: '{commandKey}'. Type 'help' for available commands.");
                    allExecuted = false;
                    continue;
                }

                List<Argument> arguments = parts.Skip(1)
                                                .Select(a => new Argument(a))
                                                .ToList();

                if (!commands[commandKey].Execute(arguments))
                    allExecuted = false;
            }

            return allExecuted;
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

        private string[] ParseArguments(string input)
        {
            List<string> result = new();
            bool inQuotes = false;
            string current = "";

            foreach (char c in input)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        result.Add(current);
                        current = "";
                    }
                    continue;
                }

                current += c;
            }

            if (!string.IsNullOrEmpty(current))
                result.Add(current);

            return result.ToArray();
        }

        private List<string> SplitCommands(string input)
        {
            List<string> commands = new();
            bool inQuotes = false;
            string current = "";

            foreach (char c in input)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    current += c; // keep quotes for argument parsing
                }
                else if (c == ';' && !inQuotes)
                {
                    if (!string.IsNullOrWhiteSpace(current))
                        commands.Add(current.Trim());
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            if (!string.IsNullOrWhiteSpace(current))
                commands.Add(current.Trim());

            return commands;
        }
    }
}