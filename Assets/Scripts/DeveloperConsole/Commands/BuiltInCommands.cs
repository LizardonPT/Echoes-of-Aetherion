using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EchoesOfEtherion.DeveloperConsole.CFG;
using EchoesOfEtherion.DeveloperConsole.Inputs;
using EchoesOfEtherion.Game.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoesOfEtherion.DeveloperConsole.Commands
{
    public class BuiltInCommands : MonoBehaviour
    {
        private void Start()
        {
            RegisterCommands();
        }

        public void RegisterCommands()
        {
            RegisterHelpCommands();

            RegisterSystemCommands();

            RegisterSceneCommands();

            RegisterPlayerCommands();

            RegisterDebugCommands();

            RegisterCFGCommands();

            RegisterBindingCommands();
        }

        private void RegisterHelpCommands()
        {
            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "help",
                description: "Shows list of available commands or help for specific command",
                usage: "help [command]",
                action: (args) =>
                {
                    if (args.Count == 0)
                    {
                        // Show all commands grouped by category
                        var commandsByCategory = CommandDatabase.Instance.GetAllCommandsByCategory();

                        foreach (var category in commandsByCategory)
                        {
                            ConsoleLogger.Log($"\n=== {category.Key.ToUpper()} ===");
                            foreach (var command in category.Value)
                            {
                                ConsoleLogger.Log($"{command.Key} - {command.Description}");
                            }
                        }
                    }
                    else
                    {
                        string commandKey = args[0].GetString();

                        if (CommandDatabase.Instance.TryGetCommand(commandKey, out IConsoleCommand command))
                        {
                            ConsoleLogger.Log($"\n=== {command.Key.ToUpper()} ===");
                            ConsoleLogger.Log($"Description: {command.Description}");
                            ConsoleLogger.Log($"Usage: {command.Usage}");
                        }
                        else
                        { // Provide info on argument types
                            if (commandKey == "bool")
                                ConsoleLogger.Log("Boolean values can be represented as: 1/0, true/false, yes/no, on/off");
                            else if (commandKey == "number")
                                ConsoleLogger.Log("Number values should be in standard decimal format as: 3.14, -42, 0.001");
                            else if (commandKey == "string")
                                ConsoleLogger.Log("String values are plain text. If spaces are needed, enclose in quotes.");
                            else
                                ConsoleLogger.Log($"Command '{commandKey}' not found");
                        }
                    }
                },
                expectedArgs: new List<Argument> { new("") }
                ), "Help");
        }

        private void RegisterSystemCommands()
        {
            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
            key: "echo",
            description: "Prints text to the console",
            usage: "echo <text>",
            action: (args) => ConsoleLogger.Log(args[0].GetString())

            ), "System");


            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "clear",
                description: "Clears the console output",
                usage: "clear",
                action: (args) => ConsoleLogger.ClearLog()

                ), "System");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "quit",
                description: "Quits the game",
                usage: "quit",
                action: (args) =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
                ), "System");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "time_scale",
                description: "Sets the game time scale",
                usage: "time_scale <value>",
                action: (arguments) =>
                {
                    if (arguments.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: time_scale <value>");
                        return;
                    }

                    if (arguments[0].TryGetNumber(out float numValue))
                    {
                        ConsoleLogger.Log($"Time scale set to {numValue}");
                        Time.timeScale = Mathf.Max(0, numValue);
                        return;
                    }
                    else
                        ConsoleLogger.Log("Error: Invalid number format");
                }
                ), "System");
        }

        private void RegisterSceneCommands()
        {
            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "change_scene",
                description: "Changes the current scene to the specified scene name",
                usage: "change_scene <sceneName>",
                action: (args) =>
                {
                    if (args.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: change_scene <sceneName>");
                        return;
                    }

                    string sceneName = args[0].GetString();

                    if (SceneManager.GetSceneByName(sceneName) == null)
                    {
                        ConsoleLogger.Log($"Error: Scene '{sceneName}' not found");
                    }
                    else
                    {
                        ConsoleLogger.Log($"Changing scene to '{sceneName}'...");
                        SceneLoader.Instance.SwitchToScene(sceneName);
                    }
                },
                expectedArgs: new List<Argument>
                {
                    new(SceneLoader.Instance.CurrentSceneName),
                }
                ), "Scene");
        }

        private void RegisterPlayerCommands()
        {
            // todo: God mode
            CommandDatabase.Instance.RegisterCommand(new SettingCommand(
                key: "godmode",
                description: "Toggles god mode (invincibility)",
                usage: "godmode <0/1>",
                setter: (value) =>
                {
                    if (!value.TryGetBoolean(out bool boolValue))
                    {
                        ConsoleLogger.Log("Error: Invalid boolean format");
                        return;
                    }
                    bool enabled = boolValue;

                    //todo: Implement god mode logic
                    ConsoleLogger.Log($"God mode {(enabled ? "enabled" : "disabled")}");
                },
                getter: () => "Check not implemented"
                ), "Player");

            //todo: NoClip mode
            CommandDatabase.Instance.RegisterCommand(new SettingCommand(
                key: "noclip",
                description: "Toggles noclip mode",
                usage: "noclip <0/1>",
                setter: (value) =>
                {
                    if (!value.TryGetBoolean(out bool boolValue))
                    {
                        ConsoleLogger.Log("Error: Invalid boolean format");
                        return;
                    }

                    bool enabled = boolValue;

                    //todo: Implement noclip logic
                    ConsoleLogger.Log($"Noclip {(enabled ? "enabled" : "disabled")}");
                },
                getter: () => "Check not implemented"
                ), "Player");
        }

        private void RegisterDebugCommands()
        {
            //todo: Log level
            CommandDatabase.Instance.RegisterCommand(new SettingCommand(
                key: "log_level",
                description: "Sets the log level (0-Error, 1-Warning, 2-Info, 3-Debug)",
                usage: "log_level <level>",
                setter: (value) =>
                {
                    if (!value.TryGetNumber(out float numValue))
                    {
                        ConsoleLogger.Log("Error: Invalid number format");
                        return;
                    }

                    int level = Mathf.Clamp(Mathf.RoundToInt(numValue), 0, 3);

                    ConsoleLogger.Log($"Log level set to {level}");
                },
                getter: () => "Current level" //todo: implement
                ), "Debug");

            // Performance stats
            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "stats",
                description: "Shows performance statistics",
                usage: "stats",
                action: (args) =>
                {
                    float fps = 1f / Time.deltaTime;
                    long memory = GC.GetTotalMemory(false) / (1024 * 1024); // MB

                    ConsoleLogger.Log($"=== PERFORMANCE STATS ===");
                    ConsoleLogger.Log($"FPS: {fps:F1}");
                    ConsoleLogger.Log($"Memory: {memory} MB");
                    ConsoleLogger.Log($"Time Scale: {Time.timeScale}");
                    ConsoleLogger.Log($"Target FPS: {(Application.targetFrameRate == -1 ? "Unlimited" : Application.targetFrameRate.ToString())}");
                }
                ), "Debug");
        }

        public void RegisterCFGCommands()
        {
            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "exec",
                description: "Executes a config file",
                usage: "exec <filename.cfg>",
                action: (args) =>
                {
                    if (args.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: exec <filename.cfg>");
                        return;
                    }

                    string fileName = args[0].GetString();
                    fileName = fileName.EndsWith(".cfg") ? fileName : $"{fileName}.cfg";
                    CFGReaderWriter.Instance.ExecuteConfigFileByName(fileName);
                },
                expectedArgs: new List<Argument> { new("") }
            ), "CFG");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "cfg_exec",
                description: "Executes a config file",
                usage: "cfg_exec <filename.cfg>",
                action: (args) =>
                {
                    if (args.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: cfg_exec <filename.cfg>");
                        return;
                    }

                    string fileName = args[0].GetString();
                    fileName = fileName.EndsWith(".cfg") ? fileName : $"{fileName}.cfg";
                    CFGReaderWriter.Instance.ExecuteConfigFileByName(fileName);
                },
                expectedArgs: new List<Argument> { new("") }
                ), "CFG");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "cfg_list",
                description: "Lists all available config files",
                usage: "cfg_list",
                action: (args) =>
                {
                    var files = CFGReaderWriter.Instance.GetAllConfigFiles();

                    if (files.Count == 0)
                    {
                        ConsoleLogger.Log("No config files found.");
                        return;
                    }

                    ConsoleLogger.Log("=== CONFIG FILES ===");
                    foreach (string file in files)
                    {
                        ConsoleLogger.Log(file);
                    }
                }
                ), "CFG");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "cfg_settings_save",
                description: "Saves current settings to config file",
                usage: "cfg_settings_save",
                action: (args) =>
                {
                    CFGReaderWriter.Instance.SaveSettings();
                },
                expectedArgs: new List<Argument> { }
                ), "CFG");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "cfg_create",
                description: "Creates a new config file with commands",
                usage: "cfg_create <filename> \"command1\" \"command2\" ...",
                action: (args) =>
                {
                    if (args.Count < 2)
                    {
                        ConsoleLogger.Log("Usage: cfg_create <filename> \"command1\" \"command2\" ...");
                        return;
                    }

                    string fileName = args[0].GetString();
                    var commands = new List<string>();

                    for (int i = 1; i < args.Count; i++)
                    {
                        commands.Add(args[i].GetString().Trim('"'));
                    }

                    fileName = fileName.EndsWith(".cfg") ? fileName : fileName + ".cfg";

                    CFGReaderWriter.Instance.CreateConfigFile(fileName, commands);
                },
                expectedArgs: new List<Argument>
                {
                    new(""),
                    new("")
                }
                ), "CFG");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "cfg_rename",
                description: "Renames a cfg file",
                usage: "cfg_rename <filename> <newfilename>",
                action: (args) =>
                {
                    if (args.Count < 2)
                    {
                        ConsoleLogger.Log("cfg_rename <filename> <newfilename>");
                        return;
                    }

                    string fileName = args[0].GetString();
                    fileName = fileName.EndsWith(".cfg") ? fileName : fileName + ".cfg";
                    string fullPath = CFGReaderWriter.Instance.GetFullPath(fileName);

                    string newFileName = args[1].GetString();
                    newFileName = newFileName.EndsWith(".cfg") ? newFileName : newFileName + ".cfg";
                    string newFullPath = Path.Combine(Path.GetDirectoryName(fullPath), newFileName);

                    if (File.Exists(fullPath))
                    {
                        if (File.Exists(newFullPath))
                        {
                            ConsoleLogger.Log($"Cannot rename: {newFileName} already exists.");
                        }
                        else
                        {
                            File.Move(fullPath, newFullPath);
                            ConsoleLogger.Log($"Renamed {fileName} → {newFileName}");
                        }
                    }
                    else
                    {
                        ConsoleLogger.Log($"File {fileName} does not exist.");
                    }
                },
                expectedArgs: new List<Argument>
                {
                    new(""),
                    new("")
                }
                ), "CFG");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "cfg_delete",
                description: "Deletes a cfg file",
                usage: "cfg_delete <filename>",
                action: (args) =>
                {
                    if (args.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: cfg_delete <filename>");
                        return;
                    }

                    string fileName = args[0].GetString();
                    fileName = fileName.EndsWith(".cfg") ? fileName : fileName + ".cfg";

                    // Protect default files
                    string[] protectedFiles = { "default_bindings.cfg", "settings.cfg" };
                    if (protectedFiles.Contains(fileName.ToLower()))
                    {
                        ConsoleLogger.Log($"Cannot delete protected file: {fileName}");
                        return;
                    }

                    string fullPath = CFGReaderWriter.Instance.GetFullPath(fileName);

                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            File.Delete(fullPath);
                            ConsoleLogger.Log($"Deleted {fileName}");
                        }
                        catch (Exception e)
                        {
                            ConsoleLogger.Log($"Failed to delete {fileName}: {e.Message}");
                        }
                    }
                    else
                    {
                        ConsoleLogger.Log($"File {fileName} does not exist.");
                    }
                },
                expectedArgs: new List<Argument>
                {
                    new( ""),
                }
                ), "CFG");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "cfg_view",
                description: "Views the contents of a config file",
                usage: "cfg_view <filename>",
                action: (args) =>
                {
                    if (args.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: cfg_view <filename>");
                        return;
                    }

                    string fileName = args[0].GetString();
                    fileName = fileName.EndsWith(".cfg") ? fileName : fileName + ".cfg";

                    var lines = CFGReaderWriter.Instance.ReadConfigFile(fileName);

                    if (lines.Count == 0)
                    {
                        ConsoleLogger.Log($"No commands found in {fileName} or file doesn't exist.");
                        return;
                    }

                    ConsoleLogger.Log($"=== {fileName.ToUpper()} ===");
                    foreach (string line in lines)
                    {
                        ConsoleLogger.Log(line);
                    }
                },
                expectedArgs: new List<Argument> { new("") }
                ), "CFG");
        }

        public void RegisterBindingCommands()
        {
            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "bind",
                description: "Bind a key to an action",
                usage: "bind <key> [action]",
                action: (args) =>
                {
                    if (args.Count < 2)
                    {
                        ConsoleLogger.Log("Usage: bind <key> [action]");
                        return;
                    }

                    string key = args[0].GetString();
                    string target = args[1].GetString();

                    InputBindingManager.Instance.Bind(target, key);
                },
                expectedArgs: new List<Argument>
                {
                    new(""),
                    new("")
                }
                ), "Input");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "unbind",
                description: "Unbind a key from an action",
                usage: "unbind <key> [action]",
                action: (args) =>
                {
                    if (args.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: unbind <key> [action]");
                        return;
                    }

                    string key = args[0].GetString();

                    if (args.Count == 1)
                    {
                        InputBindingManager.Instance.Unbind(key);
                    }
                    else
                    {
                        string target = args[1].GetString();
                        InputBindingManager.Instance.Unbind(key, target);
                    }
                },
                expectedArgs: new List<Argument>
                {
                    new(""),
                    new("")
                }
                ), "Input");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "unbind_action",
                description: "Unbind all keys from an action",
                usage: "unbind_action [action]",
                action: (args) =>
                {
                    if (args.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: unbind <key> [action]");
                        return;
                    }
                    string action = args[0].GetString();

                    InputBindingManager.Instance.UnbindAllFromAction(action);
                },
                expectedArgs: new List<Argument>
                {
                   new(""),
                   new("")
                }
                ), "Input");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "list_binds",
                description: "List all current key bindings",
                usage: "list_binds [action]",
                action: (args) =>
                {
                    if (args.Count == 0)
                    {
                        var allBindings = InputBindingManager.Instance.GetAllBindings();

                        if (allBindings.Count == 0)
                        {
                            ConsoleLogger.Log("No bindings found");
                            return;
                        }

                        ConsoleLogger.Log("=== CURRENT BINDINGS ===");
                        foreach (var kvp in allBindings.Where(b => !b.Key.StartsWith("+")))
                        {
                            string boundKeys = string.Join(", ", kvp.Value);
                            ConsoleLogger.Log($"{kvp.Key}: {boundKeys}");
                        }
                    }
                    else
                    {
                        string target = args[0].GetString();

                        // List action bindings
                        var keys = InputBindingManager.Instance.GetKeysForAction(target);

                        if (keys.Count == 0)
                        {
                            ConsoleLogger.Log($"No bindings found for action '{target}'");
                            return;
                        }

                        ConsoleLogger.Log($"=== BINDINGS FOR {target.ToUpper()} ===");
                        foreach (var key in keys)
                        {
                            ConsoleLogger.Log(key);
                        }

                    }
                },
                expectedArgs: new List<Argument>
                {
                    new( "")
                }
                ), "Input");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "find_key",
                description: "Find which action(s) a key is bound to",
                usage: "find_key <key>",
                action: (args) =>
                {
                    if (args.Count < 1)
                    {
                        ConsoleLogger.Log("Usage: find_key <key>");
                        return;
                    }

                    string key = args[0].GetString();

                    // Get all actions for this key across all maps
                    var actions = InputBindingManager.Instance.GetActionsForKey(key);

                    if (actions.Count == 0)
                    {
                        ConsoleLogger.Log($"Key '{key}' is not bound to any action");
                    }
                    else
                    {
                        string actionList = string.Join(", ", actions);
                        ConsoleLogger.Log($"Key '{key}' is bound to: {actionList}");
                    }
                },
                expectedArgs: new List<Argument>
                {
                    new("")
                }
                ), "Input");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "reset_binds",
                description: "Reset all bindings to defaults",
                usage: "reset_binds",
                action: (args) =>
                {
                    CFGReaderWriter.Instance.ExecuteDefaultBinds();
                }
            ), "Input");

            CommandDatabase.Instance.RegisterCommand(new ActionCommand(
                key: "key_list",
                description: "Show list of available key aliases",
                usage: "key_list [filter]",
                action: (args) =>
                {
                    string filter = args.Count > 0 ? args[0].GetString().ToLower() : "";

                    ConsoleLogger.Log("=== AVAILABLE KEY ALIASES ===");
                    ConsoleLogger.Log("Use these names with the 'bind' command");
                    ConsoleLogger.Log("");

                    var categories = new Dictionary<string, List<string>>();

                    foreach (var alias in InputBindingManager.Instance.KeyAliases)
                    {
                        if (!string.IsNullOrEmpty(filter) && !alias.Key.Contains(filter))
                            continue;

                        string category = "Other";
                        if (alias.Key.StartsWith("mouse"))
                            category = "Mouse";
                        else if (alias.Key.StartsWith("gamepad"))
                            category = "Gamepad";
                        else if (alias.Key.StartsWith("f") && alias.Key.Length > 1 && char.IsDigit(alias.Key[1]))
                            category = "Function Keys";
                        else if (alias.Key.StartsWith("num"))
                            category = "Numpad";
                        else if (alias.Key.Length == 1 && char.IsLetter(alias.Key[0]))
                            category = "Letters";
                        else if (alias.Key.Length == 1 && char.IsDigit(alias.Key[0]))
                            category = "Numbers";
                        else if (new[] { "up", "down", "left", "right" }.Contains(alias.Key))
                            category = "Arrow Keys";
                        else if (new[] { "shift", "ctrl", "alt", "space", "enter", "tab", "escape" }.Contains(alias.Key))
                            category = "Special Keys";

                        if (!categories.ContainsKey(category))
                            categories[category] = new List<string>();

                        categories[category].Add(alias.Key);
                    }

                    foreach (var category in categories.OrderBy(c => c.Key))
                    {
                        ConsoleLogger.Log($"{category.Key}:");
                        foreach (var key in category.Value.OrderBy(k => k))
                        {
                            ConsoleLogger.Log($"  {key}");
                        }
                        ConsoleLogger.Log("");
                    }
                },
                expectedArgs: new List<Argument>
                {
                    new("")
                }
            ), "Input");
        }
    }
}