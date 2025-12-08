using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EchoesOfEtherion.DeveloperConsole.CFG;
using EchoesOfEtherion.DeveloperConsole.Commands;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace EchoesOfEtherion.DeveloperConsole.Inputs
{
    public class InputBindingManager : Singleton<InputBindingManager>
    {
        [Header("Input Settings")]
        [SerializeField] private InputActionAsset inputActions;

        private readonly Dictionary<string, List<InputBinding>> actionBindings = new();
        private readonly Dictionary<(string mapName, string key), string> keyToActionCache = new();
        private readonly Dictionary<string, string> keyAliases = new();

        // Command binding support
        private readonly Dictionary<string, List<string>> commandBindings = new();
        private readonly Dictionary<string, InputControl> commandKeyControls = new();
        private readonly Dictionary<string, bool> commandKeyPressedState = new();

        public Dictionary<string, string> KeyAliases => new(keyAliases);
        public event Action<string, string> BindingChanged; // actionName, bindingPath
        public event Action<string> BindingRemoved; // bindingPath

        protected override void Awake()
        {
            base.Awake();
            InitializeKeyAliases();
            CacheExistingBindings();
        }

        private void InitializeKeyAliases()
        {
            // Mouse buttons
            keyAliases["mouse1"] = "<Mouse>/leftButton";
            keyAliases["mouse2"] = "<Mouse>/rightButton";
            keyAliases["mouse3"] = "<Mouse>/middleButton";
            keyAliases["mouse4"] = "<Mouse>/forwardButton";
            keyAliases["mouse5"] = "<Mouse>/backButton";
            keyAliases["mousewheelup"] = "<Mouse>/scroll/up";
            keyAliases["mousewheeldown"] = "<Mouse>/scroll/down";

            // Keyboard letters
            for (char c = 'a'; c <= 'z'; c++)
                keyAliases[c.ToString()] = $"<Keyboard>/{c}";

            // Keyboard numbers
            for (int i = 0; i <= 9; i++)
                keyAliases[i.ToString()] = $"<Keyboard>/{i}";

            // Function keys
            for (int i = 1; i <= 12; i++)
                keyAliases[$"f{i}"] = $"<Keyboard>/f{i}";

            // Special keys
            keyAliases["space"] = "<Keyboard>/space";
            keyAliases["enter"] = "<Keyboard>/enter";
            keyAliases["tab"] = "<Keyboard>/tab";
            keyAliases["capslock"] = "<Keyboard>/capsLock";
            keyAliases["shift"] = "<Keyboard>/shift";
            keyAliases["lshift"] = "<Keyboard>/leftShift";
            keyAliases["rshift"] = "<Keyboard>/rightShift";
            keyAliases["ctrl"] = "<Keyboard>/ctrl";
            keyAliases["lctrl"] = "<Keyboard>/leftCtrl";
            keyAliases["rctrl"] = "<Keyboard>/rightCtrl";
            keyAliases["alt"] = "<Keyboard>/alt";
            keyAliases["lalt"] = "<Keyboard>/leftAlt";
            keyAliases["ralt"] = "<Keyboard>/rightAlt";
            keyAliases["windows"] = "<Keyboard>/leftWindows";
            keyAliases["rwindows"] = "<Keyboard>/rightWindows";
            keyAliases["escape"] = "<Keyboard>/escape";
            keyAliases["backspace"] = "<Keyboard>/backspace";
            keyAliases["delete"] = "<Keyboard>/delete";
            keyAliases["insert"] = "<Keyboard>/insert";
            keyAliases["home"] = "<Keyboard>/home";
            keyAliases["end"] = "<Keyboard>/end";
            keyAliases["pageup"] = "<Keyboard>/pageUp";
            keyAliases["pagedown"] = "<Keyboard>/pageDown";

            // Arrow keys
            keyAliases["uparrow"] = "<Keyboard>/upArrow";
            keyAliases["downarrow"] = "<Keyboard>/downArrow";
            keyAliases["leftarrow"] = "<Keyboard>/leftArrow";
            keyAliases["rightarrow"] = "<Keyboard>/rightArrow";

            // Numpad
            for (int i = 0; i <= 9; i++)
                keyAliases[$"numpad{i}"] = $"<Keyboard>/numpad{i}";

            keyAliases["numlock"] = "<Keyboard>/numLock";
            keyAliases["numenter"] = "<Keyboard>/numpadEnter";
            keyAliases["numplus"] = "<Keyboard>/numpadPlus";
            keyAliases["numminus"] = "<Keyboard>/numpadMinus";
            keyAliases["nummultiply"] = "<Keyboard>/numpadMultiply";
            keyAliases["numdivide"] = "<Keyboard>/numpadDivide";

            keyAliases["backquote"] = "<Keyboard>/backquote";
            keyAliases["semicolon"] = "<Keyboard>/semicolon";
            keyAliases["quote"] = "<Keyboard>/quote";
            keyAliases["comma"] = "<Keyboard>/comma";
            keyAliases["period"] = "<Keyboard>/period";
            keyAliases["slash"] = "<Keyboard>/slash";
            keyAliases["backslash"] = "<Keyboard>/backslash";
            keyAliases["minus"] = "<Keyboard>/minus";
            keyAliases["equals"] = "<Keyboard>/equals";
            keyAliases["leftbracket"] = "<Keyboard>/leftBracket";
            keyAliases["rightbracket"] = "<Keyboard>/rightBracket";
        }

        private void CacheExistingBindings()
        {
            actionBindings.Clear();
            keyToActionCache.Clear();

            if (inputActions == null)
                return;

            foreach (var actionMap in inputActions.actionMaps)
            {
                string mapName = actionMap.name;
                foreach (var action in actionMap.actions)
                {
                    string actionName = action.name;
                    var bindings = action.bindings.ToList();

                    actionBindings[actionName] = bindings;

                    // Cache key to action mapping per map
                    foreach (var binding in bindings)
                    {
                        if (!string.IsNullOrEmpty(binding.path))
                        {
                            string key = GetKeyAlias(binding.path, false);
                            if (!string.IsNullOrEmpty(key))
                            {
                                keyToActionCache[(mapName, key.ToLower())] = actionName;
                            }
                        }
                    }
                }
            }
        }

        public string GetKeyAlias(string inputPath, bool toAlias = true)
        {
            if (toAlias)
            {
                // Convert Unity path to user-friendly alias
                foreach (var alias in keyAliases)
                {
                    if (alias.Value.Equals(inputPath, StringComparison.OrdinalIgnoreCase))
                        return alias.Key;
                }

                // If no alias found, return the original path
                return inputPath;
            }
            else
            {
                // Convert user-friendly alias to Unity path
                if (keyAliases.TryGetValue(inputPath.ToLower(), out string path))
                    return path;

                // If not found in aliases, assume it's already a Unity path
                return inputPath;
            }
        }

        public bool Bind(string target, string keyAlias)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(keyAlias))
                return false;

            if (!keyAliases.ContainsKey(keyAlias.ToLower()))
            {
                ConsoleLogger.Log($"Key alias '{keyAlias}' is not recognized.");
                ConsoleLogger.Log($"Use command 'key_list' to see all available key aliases.");
                return false;
            }

            // Check if target is an input action first
            var action = FindInputAction(target);
            if (action != null)
            {
                // Bind to input action
                return BindInputAction(target, keyAlias);
            }
            else
            {
                // Bind to command
                return BindCommand(target, keyAlias);
            }
        }

        private bool BindInputAction(string actionName, string keyAlias)
        {
            var action = FindInputAction(actionName);
            if (action == null)
                return false;

            string mapName = action.actionMap.name;

            // Check if key is already bound in the same map
            string existingAction = GetActionForKey(keyAlias, mapName);
            if (!string.IsNullOrEmpty(existingAction) && existingAction != actionName)
            {
                UnbindKeyFromAction(keyAlias, existingAction);
            }

            // Remove any existing binding for this action with same key
            UnbindKeyFromAction(keyAlias, actionName);

            string bindingPath = GetKeyAlias(keyAlias, false);
            var newBinding = new InputBinding
            {
                path = bindingPath,
                action = actionName,
                name = $"Bind_{keyAlias}"
            };

            action.AddBinding(newBinding);

            // Update cache
            keyToActionCache[(mapName, keyAlias.ToLower())] = actionName;
            BindingChanged?.Invoke(actionName, bindingPath);
            UpdateCacheForAction(actionName);
            ConsoleLogger.Log($"Bound {keyAlias} to input action '{actionName}' in map {mapName}");
            return true;
        }

        private bool BindCommand(string command, string keyAlias)
        {
            keyAlias = keyAlias.ToLower();

            // Strip quotes from the command if present
            string cleanCommand = command.Trim('"');

            if (string.IsNullOrEmpty(cleanCommand))
            {
                ConsoleLogger.Log($"Command cannot be empty");
                return false;
            }

            string bindingPath = GetKeyAlias(keyAlias, false);

            if (!commandBindings.ContainsKey(keyAlias))
            {
                commandBindings[keyAlias] = new List<string>();
            }

            // Check if this command is already bound to this key
            if (commandBindings[keyAlias].Contains(cleanCommand))
            {
                ConsoleLogger.Log($"Command '{cleanCommand}' is already bound to key '{keyAlias}'");
                return false;
            }

            commandBindings[keyAlias].Add(cleanCommand);

            // Cache the InputControl for this key
            var control = InputSystem.FindControl(bindingPath);
            if (control != null && !commandKeyControls.ContainsKey(keyAlias))
            {
                commandKeyControls[keyAlias] = control;
            }

            ConsoleLogger.Log($"Bound key '{keyAlias}' to command: {cleanCommand}");
            return true;
        }

        public bool Unbind(string keyAlias, string target = null)
        {
            if (string.IsNullOrEmpty(keyAlias))
                return false;

            bool inputActionUnbound = false;
            bool commandUnbound = false;

            // Handle input action unbinding
            if (string.IsNullOrEmpty(target))
            {
                inputActionUnbound = UnbindKeyFromAllActions(keyAlias);
                commandUnbound = UnbindCommand(keyAlias);
            }
            else
            {
                // Try to unbind from input actions first
                inputActionUnbound = UnbindKeyFromAction(keyAlias, target);

                // If not an input action, try command unbinding
                if (!inputActionUnbound)
                {
                    // Strip quotes from target if present for command comparison
                    string cleanTarget = target.Trim('"');
                    commandUnbound = UnbindCommand(keyAlias, cleanTarget);
                }
            }

            return inputActionUnbound || commandUnbound;
        }

        private bool UnbindKeyFromAction(string keyAlias, string actionName)
        {
            var action = FindInputAction(actionName);
            if (action == null)
                return false;

            string mapName = action.actionMap.name;
            string bindingPath = GetKeyAlias(keyAlias, false);
            bool removed = false;

            // Find matching bindings
            var bindingsToRemove = action.bindings
                .Where(b => b.path.Equals(bindingPath, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var binding in bindingsToRemove)
            {
                int index = action.bindings.IndexOf(b => b == binding);
                if (index != -1)
                {
                    action.ChangeBinding(index).Erase();
                    removed = true;
                }
            }

            if (removed)
            {
                UpdateCacheForAction(actionName);
                keyToActionCache.Remove((mapName, keyAlias.ToLower()));
                BindingRemoved?.Invoke(bindingPath);
                ConsoleLogger.Log($"Unbound {keyAlias} from input action '{actionName}' in map {mapName}");
            }

            return removed;
        }

        private bool UnbindKeyFromAllActions(string keyAlias)
        {
            bool removed = false;
            var keysToRemove = keyToActionCache.Keys
                .Where(k => k.Item2.Equals(keyAlias, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var (mapName, key) in keysToRemove)
            {
                string actionName = keyToActionCache[(mapName, key)];
                if (UnbindKeyFromAction(keyAlias, actionName))
                    removed = true;
            }

            if (!removed)
                ConsoleLogger.Log($"Key {keyAlias} is not bound to any input action");

            return removed;
        }

        private bool UnbindCommand(string keyAlias, string command = null)
        {
            keyAlias = keyAlias.ToLower();

            if (string.IsNullOrEmpty(command))
            {
                // Remove all command bindings for this key
                if (commandBindings.Remove(keyAlias))
                {
                    commandKeyControls.Remove(keyAlias);
                    commandKeyPressedState.Remove(keyAlias);
                    ConsoleLogger.Log($"Removed all command bindings for key '{keyAlias}'");
                    return true;
                }
                return false;
            }
            else
            {
                // Strip quotes from command if present
                string cleanCommand = command.Trim('"');

                // Remove specific command binding
                if (commandBindings.TryGetValue(keyAlias, out var commands))
                {
                    if (commands.Remove(cleanCommand))
                    {
                        if (commands.Count == 0)
                        {
                            commandBindings.Remove(keyAlias);
                            commandKeyControls.Remove(keyAlias);
                            commandKeyPressedState.Remove(keyAlias);
                        }
                        ConsoleLogger.Log($"Unbound key '{keyAlias}' from command '{cleanCommand}'");
                        return true;
                    }
                }
                ConsoleLogger.Log($"Key '{keyAlias}' is not bound to command '{cleanCommand}'");
                return false;
            }
        }

        public void UnbindAllFromAction(string actionName)
        {
            // For input actions
            var action = FindInputAction(actionName);
            if (action != null)
            {
                string mapName = action.actionMap.name;
                var bindingsToRemove = action.bindings.ToList();

                foreach (var binding in bindingsToRemove)
                {
                    if (!string.IsNullOrEmpty(binding.path))
                    {
                        int index = action.bindings.IndexOf(b => b == binding);
                        if (index != -1)
                        {
                            action.ChangeBinding(index).Erase();
                            string keyAlias = GetKeyAlias(binding.path, true);
                            keyToActionCache.Remove((mapName, keyAlias.ToLower()));
                        }
                    }
                }

                UpdateCacheForAction(actionName);
                ConsoleLogger.Log($"Removed all bindings from input action '{actionName}' in map {mapName}");
            }

            // Also check if it's a command binding and remove all keys bound to it
            foreach (var kvp in commandBindings.ToList())
            {
                if (kvp.Value.Contains(actionName))
                {
                    UnbindCommand(kvp.Key, actionName);
                }
            }
        }

        public string GetActionForKey(string keyAlias, string mapName)
        {
            keyAlias = keyAlias.ToLower();
            return keyToActionCache.TryGetValue((mapName, keyAlias), out string action) ? action : null;
        }

        public List<string> GetActionsForKey(string keyAlias)
        {
            keyAlias = keyAlias.ToLower();
            var actions = new List<string>();

            // Get input action bindings
            var inputActionsList = keyToActionCache
                .Where(kvp => kvp.Key.key == keyAlias)
                .Select(kvp => kvp.Value)
                .Distinct()
                .ToList();

            actions.AddRange(inputActionsList);

            // Get command bindings
            if (commandBindings.TryGetValue(keyAlias, out var commands))
            {
                // Format commands with quotes to distinguish them from input actions
                actions.AddRange(commands.Select(c => $"\"{c}\""));
            }

            return actions;
        }

        public List<string> GetKeysForAction(string actionName)
        {
            var keys = new List<string>();

            // Get keys for input actions
            if (actionBindings.TryGetValue(actionName, out var bindings))
            {
                foreach (var binding in bindings)
                {
                    if (!string.IsNullOrEmpty(binding.path))
                    {
                        string alias = GetKeyAlias(binding.path, true);
                        if (!string.IsNullOrEmpty(alias))
                            keys.Add(alias);
                    }
                }
            }

            // Get keys for command bindings
            foreach (var kvp in commandBindings)
            {
                if (kvp.Value.Contains(actionName))
                {
                    keys.Add(kvp.Key);
                }
            }

            return keys;
        }

        public Dictionary<string, List<string>> GetAllBindings()
        {
            var allBindings = new Dictionary<string, List<string>>();

            // Input action bindings
            foreach (var kvp in actionBindings)
            {
                var keys = GetKeysForAction(kvp.Key);
                if (keys.Count > 0)
                    allBindings[kvp.Key] = keys;
            }

            // Command bindings
            foreach (var kvp in commandBindings)
            {
                foreach (var command in kvp.Value)
                {
                    if (!allBindings.ContainsKey(command))
                        allBindings[command] = new List<string>();

                    if (!allBindings[command].Contains(kvp.Key))
                        allBindings[command].Add(kvp.Key);
                }
            }

            return allBindings;
        }

        public Dictionary<string, List<string>> GetAllCommandBindings()
        {
            return new Dictionary<string, List<string>>(commandBindings);
        }

        private InputAction FindInputAction(string actionName)
        {
            if (inputActions == null)
                return null;

            // Try to find action in any action map
            foreach (var actionMap in inputActions.actionMaps)
            {
                var action = actionMap.FindAction(actionName);
                if (action != null)
                    return action;
            }

            return null;
        }

        private void UpdateCacheForAction(string actionName)
        {
            var action = FindInputAction(actionName);
            if (action == null)
                return;

            string mapName = action.actionMap.name;

            // Remove all keys for this action in this map
            var keysToRemove = keyToActionCache
                .Where(kvp => kvp.Value == actionName && kvp.Key.mapName == mapName)
                .Select(k => k.Key.key)
                .ToList();

            foreach (var key in keysToRemove)
                keyToActionCache.Remove((mapName, key));

            // Update actionBindings cache
            actionBindings[actionName] = action.bindings.ToList();

            // Add current bindings
            foreach (var binding in action.bindings)
            {
                if (!string.IsNullOrEmpty(binding.path))
                {
                    string alias = GetKeyAlias(binding.path, true);
                    keyToActionCache[(mapName, alias.ToLower())] = actionName;
                }
            }
        }

        private void Update()
        {
            if (ConsoleController.Instance.IsOpen) return;

            // Check for pressed keys with command bindings
            foreach (var kvp in commandKeyControls.ToList())
            {
                string keyAlias = kvp.Key;
                var control = kvp.Value;

                // If control became null, try to find it again
                if (control == null)
                {
                    string bindingPath = GetKeyAlias(keyAlias, false);
                    control = InputSystem.FindControl(bindingPath);
                    if (control == null)
                    {
                        // Remove from cache if we can't find the control
                        commandKeyControls.Remove(keyAlias);
                        continue;
                    }
                    commandKeyControls[keyAlias] = control;
                }

                if (control.IsPressed())
                {
                    // Check if this is a new press (not held)
                    if (ShouldTriggerCommand(keyAlias))
                    {
                        ExecuteCommandBinding(keyAlias);
                    }
                }
                else
                {
                    // Reset the state when key is released
                    ResetCommandKeyState(keyAlias);
                }
            }
        }

        private bool ShouldTriggerCommand(string keyAlias)
        {
            // Only trigger on key down, not while held
            if (!commandKeyPressedState.ContainsKey(keyAlias))
            {
                commandKeyPressedState[keyAlias] = true;
                return true;
            }

            return false;
        }

        private void ResetCommandKeyState(string keyAlias)
        {
            commandKeyPressedState.Remove(keyAlias);
        }

        private void ExecuteCommandBinding(string keyAlias)
        {
            if (commandBindings.TryGetValue(keyAlias, out var commands))
            {
                foreach (var command in commands)
                {
                    // The command should already be clean (no quotes)
                    // But let's ensure it's clean before execution
                    string cleanCommand = command.Trim('"');
                    if (!string.IsNullOrEmpty(cleanCommand))
                    {
                        // Execute the command through the console system
                        CommandDatabase.Instance.ExecuteCommand(cleanCommand);
                    }
                }
            }
        }

        // Save/Load command bindings
        public void SaveCommandBindings()
        {
            var lines = new List<string>
            {
                "// Auto-generated command bindings",
                "// Generated by Echoes of Etherion",
                ""
            };

            foreach (var kvp in commandBindings)
            {
                foreach (var command in kvp.Value)
                {
                    // Only add quotes if the command contains spaces
                    string formattedCommand = command.Contains(' ') ? $"\"{command}\"" : command;
                    lines.Add($"bind {kvp.Key} {formattedCommand}");
                }
            }

            string filePath = Path.Combine(Application.persistentDataPath, "command_bindings.cfg");
            try
            {
                File.WriteAllLines(filePath, lines);
                ConsoleLogger.Log($"Command bindings saved to {filePath}");
            }
            catch (Exception e)
            {
                ConsoleLogger.Log($"Error saving command bindings: {e.Message}");
            }
        }

        public void LoadCommandBindings()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "command_bindings.cfg");
            if (File.Exists(filePath))
            {
                try
                {
                    // Clear existing command bindings
                    foreach (var key in commandBindings.Keys.ToList())
                    {
                        UnbindCommand(key);
                    }

                    // Execute the bind commands from the file
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string trimmedLine = line.Trim();
                        if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith("//"))
                        {
                            CommandDatabase.Instance.ExecuteCommand(trimmedLine);
                        }
                    }

                    ConsoleLogger.Log("Command bindings loaded");
                }
                catch (Exception e)
                {
                    ConsoleLogger.Log($"Error loading command bindings: {e.Message}");
                }
            }
        }
    }
}
