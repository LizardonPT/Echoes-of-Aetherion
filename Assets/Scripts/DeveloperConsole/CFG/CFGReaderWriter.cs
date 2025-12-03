using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EchoesOfEtherion.DeveloperConsole.Commands;
using EchoesOfEtherion.DeveloperConsole.Inputs;
using UnityEngine;

namespace EchoesOfEtherion.DeveloperConsole.CFG
{
    [RequireComponent(typeof(InputBindingManager))]
    public class CFGReaderWriter : Singleton<CFGReaderWriter>
    {
        private const string settingsFileName = "settings.cfg";
        private const string autoExecFileName = "autoexec.cfg";
        private const string configsFolder = "cfg";

        [Header("Auto Execution")]
        [SerializeField] private bool autoLoadSettingsOnStart = true;
        [SerializeField] private bool autoSaveSettingsOnQuit = true;
        [SerializeField] private bool executeAutoExecOnStart = true;

        private readonly Dictionary<string, SettingCommand> persistentSettings = new();
        private string configsPath;
        private InputBindingManager inputBindingManager;

        protected override void Awake()
        {
            base.Awake();

            configsPath = Path.Combine(Application.persistentDataPath, configsFolder);
            inputBindingManager = GetComponent<InputBindingManager>();
            InitializePaths();
            RegisterPersistentSettings();
        }

        private void Start()
        {
            if (executeAutoExecOnStart)
                ExecuteAutoExec();

            if (autoLoadSettingsOnStart)
                LoadSettings();
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            if (autoSaveSettingsOnQuit)
                SaveSettings();
        }

        private void InitializePaths()
        {
            // Create directory if it doesn't exist
            if (!Directory.Exists(configsPath))
            {
                Directory.CreateDirectory(configsPath);
                ConsoleLogger.Log($"Created configs directory: {configsPath}");
            }
        }

        private void RegisterPersistentSettings()
        {
            // Get all commands from database
            var allCommands = CommandDatabase.Instance.GetAllCommands();

            foreach (var command in allCommands)
            {
                if (command is SettingCommand settingCommand)
                {
                    // Only register if marked as persistent
                    if (settingCommand.IsPersistent)
                    {
                        persistentSettings[command.Key.ToLower()] = settingCommand;
                        ConsoleLogger.Log($"Registered persistent setting: {command.Key} ({settingCommand.Category})");
                    }
                }
            }
        }

        public string GetFullPath(string fileName)
        {
            return Path.Combine(configsPath, fileName);
        }

        public bool ConfigFileExists(string fileName)
        {
            string fullPath = GetFullPath(fileName);
            return File.Exists(fullPath);
        }

        public List<string> GetAllConfigFiles()
        {
            var files = new List<string>();

            if (Directory.Exists(configsPath))
            {
                foreach (string file in Directory.GetFiles(configsPath, "*.cfg"))
                {
                    files.Add(Path.GetFileName(file));
                }
            }

            return files;
        }

        public void SaveSettings()
        {
            string fileName = settingsFileName;
            string filePath = GetFullPath(fileName);

            try
            {
                using (StreamWriter writer = new(filePath, false, Encoding.UTF8))
                {
                    // Write header
                    writer.WriteLine($"// {Application.productName} Settings Configuration");
                    writer.WriteLine($"// Generated on: {DateTime.Now}");
                    writer.WriteLine();

                    // Group settings by category
                    Dictionary<string, List<KeyValuePair<string, SettingCommand>>> categorizedSettings =
                        new();

                    foreach (var kvp in persistentSettings)
                    {
                        if (kvp.Value.IsPersistent)
                        {
                            string category = kvp.Value.Category ?? "General";

                            if (!categorizedSettings.ContainsKey(category))
                                categorizedSettings[category] = new List<KeyValuePair<string, SettingCommand>>();

                            categorizedSettings[category].Add(kvp);
                        }
                    }

                    // Write by category
                    foreach (var category in categorizedSettings.Keys)
                    {
                        writer.WriteLine($"// === {category.ToUpper()} ===");

                        foreach (var kvp in categorizedSettings[category])
                        {
                            try
                            {
                                var getter = kvp.Value.Getter;
                                string currentValue = getter?.Invoke();

                                if (currentValue != null)
                                    writer.WriteLine($"{kvp.Key} {currentValue}");
                            }
                            catch (Exception e)
                            {
                                writer.WriteLine($"// Error getting value for {kvp.Key}: {e.Message}");
                            }
                        }

                        writer.WriteLine();
                    }

                    if (inputBindingManager != null)
                    {
                        writer.WriteLine();
                        writer.WriteLine("// === INPUT BINDINGS ===");

                        var allBindings = inputBindingManager.GetAllBindings();
                        foreach (var kvp in allBindings)
                        {
                            string actionName = kvp.Key;
                            foreach (var key in kvp.Value)
                            {
                                string keyAlias = GetKeyAlias(key);
                                writer.WriteLine($"bind {keyAlias} {actionName.ToLower()}");
                            }
                        }

                        writer.WriteLine();
                    }

                    // Write footer with execution info
                    writer.WriteLine($"// Total persistent settings: {categorizedSettings.Sum(c => c.Value.Count)}");
                    writer.WriteLine($"// File automatically generated by {Application.productName}");
                }

                ConsoleLogger.Log($"Saved settings to: {filePath}");
            }
            catch (Exception e)
            {
                ConsoleLogger.Log($"Error saving settings: {e.Message}");
            }
        }


        public void LoadSettings(string customFileName = null)
        {
            string fileName = customFileName ?? settingsFileName;
            string filePath = GetFullPath(fileName);

            if (!File.Exists(filePath))
            {
                ConsoleLogger.Log($"Settings file not found: {filePath}");
                return;
            }

            try
            {
                ConsoleLogger.Log($"Loading settings from: {filePath}");
                ExecuteConfigFile(filePath);
            }
            catch (Exception e)
            {
                ConsoleLogger.Log($"Error loading settings: {e.Message}");
            }
        }

        public void ExecuteAutoExec()
        {
            string filePath = GetFullPath(autoExecFileName);

            if (!File.Exists(filePath))
                return;

            try
            {
                ConsoleLogger.Log($"Executing autoexec: {filePath}");
                ExecuteConfigFile(filePath);
            }
            catch (Exception e)
            {
                ConsoleLogger.Log($"Error executing autoexec: {e.Message}");
            }
        }

        public void ExecuteDefaultBinds()
        {
            string defaultFile = "default_binds.cfg";
            string filePath = Path.Combine(Application.streamingAssetsPath, "cfg", defaultFile);

            if (File.Exists(filePath))
            {

                int lineNumber = 0;
                int commandsExecuted = 0;

                try
                {
                    string[] lines = File.ReadAllLines(filePath);

                    foreach (string line in lines)
                    {
                        lineNumber++;
                        string trimmedLine = line.Trim();

                        // Skip empty lines and comments
                        if (string.IsNullOrEmpty(trimmedLine) ||
                            trimmedLine.StartsWith("//") ||
                            trimmedLine.StartsWith("#") ||
                            trimmedLine.StartsWith(";"))
                        {
                            continue;
                        }

                        bool success = CommandDatabase.Instance.ExecuteCommand(trimmedLine);

                        if (success)
                            commandsExecuted++;
                        else
                            ConsoleLogger.Log($"Error on line {lineNumber}: {trimmedLine}");
                    }

                    ConsoleLogger.Log($"Executed {commandsExecuted} commands from {Path.GetFileName(filePath)}");
                }
                catch (Exception e)
                {
                    ConsoleLogger.Log($"Error reading config file at line {lineNumber}: {e.Message}");
                }
            }
        }

        public void ExecuteConfigFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                ConsoleLogger.Log($"Config file not found: {filePath}");
                return;
            }

            int lineNumber = 0;
            int commandsExecuted = 0;

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    lineNumber++;
                    string trimmedLine = line.Trim();

                    // Skip empty lines and comments
                    if (string.IsNullOrEmpty(trimmedLine) ||
                        trimmedLine.StartsWith("//") ||
                        trimmedLine.StartsWith("#") ||
                        trimmedLine.StartsWith(";"))
                    {
                        continue;
                    }

                    // Special handling for bind command to ensure it goes through InputBindingManager
                    if (trimmedLine.StartsWith("bind ", StringComparison.OrdinalIgnoreCase))
                    {
                        // Parse bind command
                        string bindArgs = trimmedLine.Substring(5).Trim();
                        string[] parts = bindArgs.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 2)
                        {
                            string key = parts[0];
                            string action = parts[1];

                            if (InputBindingManager.Instance != null)
                            {
                                if (InputBindingManager.Instance.Bind(action, key))
                                    commandsExecuted++;
                                else
                                    ConsoleLogger.Log($"Error on line {lineNumber}: Could not bind {key} to {action}");
                            }
                            else
                            {
                                ConsoleLogger.Log($"Error on line {lineNumber}: InputBindingManager not available");
                            }
                        }
                        else
                        {
                            ConsoleLogger.Log($"Error on line {lineNumber}: Invalid bind command format");
                        }
                    }
                    else if (trimmedLine.StartsWith("unbind ", StringComparison.OrdinalIgnoreCase))
                    {
                        // Parse unbind command
                        string unbindArgs = trimmedLine.Substring(7).Trim();
                        string[] parts = unbindArgs.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 1)
                        {
                            string key = parts[0];
                            string action = parts.Length > 1 ? parts[1] : null;

                            if (InputBindingManager.Instance != null)
                            {
                                if (InputBindingManager.Instance.Unbind(key, action))
                                    commandsExecuted++;
                                else
                                    ConsoleLogger.Log($"Error on line {lineNumber}: Could not unbind {key}");
                            }
                        }
                    }
                    else
                    {
                        // Normal command execution
                        bool success = CommandDatabase.Instance.ExecuteCommand(trimmedLine);

                        if (success)
                            commandsExecuted++;
                        else
                            ConsoleLogger.Log($"Error on line {lineNumber}: {trimmedLine}");
                    }
                }

                ConsoleLogger.Log($"Executed {commandsExecuted} commands from {System.IO.Path.GetFileName(filePath)}");
            }
            catch (Exception e)
            {
                ConsoleLogger.Log($"Error reading config file at line {lineNumber}: {e.Message}");
            }
        }

        public void ExecuteConfigFileByName(string fileName)
        {
            string filePath = GetFullPath(fileName);
            ExecuteConfigFile(filePath);
        }

        public void CreateConfigFile(string fileName, List<string> commands, string description = "")
        {
            string filePath = GetFullPath(fileName);

            try
            {
                using (StreamWriter writer = new(filePath, false, Encoding.UTF8))
                {
                    // Write header
                    writer.WriteLine($"// {Application.productName} Configuration File");
                    writer.WriteLine($"// Created: {DateTime.Now}");
                    if (!string.IsNullOrEmpty(description))
                        writer.WriteLine($"// Description: {description}");
                    writer.WriteLine();

                    // Write commands
                    foreach (string command in commands)
                    {
                        writer.WriteLine(command);
                    }
                }

                ConsoleLogger.Log($"Created config file: {filePath}");
            }
            catch (Exception e)
            {
                ConsoleLogger.Log($"Error creating config file: {e.Message}");
            }
        }

        public List<string> ReadConfigFile(string fileName)
        {
            string filePath = GetFullPath(fileName);
            List<string> lines = new List<string>();

            if (!File.Exists(filePath))
            {
                ConsoleLogger.Log($"Config file not found: {filePath}");
                return lines;
            }

            try
            {
                string[] fileLines = File.ReadAllLines(filePath);

                foreach (string line in fileLines)
                {
                    string trimmedLine = line.Trim();

                    // Skip empty lines and comments
                    if (!string.IsNullOrEmpty(trimmedLine) &&
                        !trimmedLine.StartsWith("//") &&
                        !trimmedLine.StartsWith("#") &&
                        !trimmedLine.StartsWith(";"))
                    {
                        lines.Add(trimmedLine);
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleLogger.Log($"Error reading config file: {e.Message}");
            }

            return lines;
        }

        private string GetKeyAlias(string inputPath)
        {
            if (string.IsNullOrEmpty(inputPath))
                return inputPath;

            inputPath = inputPath.ToLower();

            // Mouse buttons
            if (inputPath.StartsWith("<mouse>/"))
            {
                string button = inputPath.Substring(8); // remove "<Mouse>/"
                return button switch
                {
                    "leftbutton" => "mouse1",
                    "rightbutton" => "mouse2",
                    "middlebutton" => "mouse3",
                    "forwardbutton" => "mouse4",
                    "backbutton" => "mouse5",
                    "scroll/up" => "mousewheelup",
                    "scroll/down" => "mousewheeldown",
                    _ => $"mouse_{button}"
                };
            }

            if (inputPath.StartsWith("<keyboard>/"))
            {
                string key = inputPath.Substring(11); // remove "<Keyboard>/"

                // Arrow keys
                if (key == "uparrow") return "up";
                if (key == "downarrow") return "down";
                if (key == "leftarrow") return "left";
                if (key == "rightarrow") return "right";

                // Modifiers
                if (key == "leftshift") return "lshift";
                if (key == "rightshift") return "rshift";
                if (key == "leftctrl") return "lctrl";
                if (key == "rightctrl") return "rctrl";
                if (key == "leftalt") return "lalt";
                if (key == "rightalt") return "ralt";

                // Everything else: just remove <Keyboard>/ part
                return key;
            }

            return inputPath;
        }
    }
}