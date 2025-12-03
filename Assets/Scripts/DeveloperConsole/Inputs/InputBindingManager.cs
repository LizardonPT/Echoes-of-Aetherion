using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EchoesOfEtherion.DeveloperConsole.CFG;
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

        public bool Bind(string actionName, string keyAlias)
        {
            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(keyAlias))
                return false;

            if (keyAliases.ContainsKey(keyAlias.ToLower()) == false)
            {
                ConsoleLogger.Log($"Key alias '{keyAlias}' is not recognized.");
                ConsoleLogger.Log($"Use command 'key_list' to see all available key aliases.");
                return false;
            }

            var action = FindInputAction(actionName);
            if (action == null)
            {
                ConsoleLogger.Log($"Action '{actionName}' not found.");
                return false;
            }

            string mapName = action.actionMap.name;

            // Check if key is already bound in the same map
            string existingAction = GetActionForKey(keyAlias, mapName);

            if (!string.IsNullOrEmpty(existingAction) && existingAction != actionName)
            {
                UnbindKeyFromAction(keyAlias, existingAction); // only affects this map
            }

            // Remove any existing binding for this action with same key
            UnbindKeyFromAction(keyAlias, actionName);

            string bindingPath = GetKeyAlias(keyAlias, false);
            var newBinding = new InputBinding { path = bindingPath, action = actionName, name = $"Bind_{keyAlias}" };
            action.AddBinding(newBinding);

            // Update cache
            keyToActionCache[(mapName, keyAlias.ToLower())] = actionName;
            BindingChanged?.Invoke(actionName, bindingPath);
            UpdateCacheForAction(actionName);
            ConsoleLogger.Log($"Bound {keyAlias} to {actionName} in map {mapName}");
            return true;
        }


        public bool Unbind(string keyAlias, string actionName = null)
        {
            if (string.IsNullOrEmpty(keyAlias))
                return false;

            if (string.IsNullOrEmpty(actionName))
            {
                // Unbind from all actions
                return UnbindKeyFromAllActions(keyAlias);
            }
            else
            {
                // Unbind from specific action
                return UnbindKeyFromAction(keyAlias, actionName);
            }
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
            var bindingsToRemove = action.bindings.Where(b => b.path.Equals(bindingPath, StringComparison.OrdinalIgnoreCase)).ToList();

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
                ConsoleLogger.Log($"Unbound {keyAlias} from {actionName} in map {mapName}");
            }

            return removed;
        }

        private bool UnbindKeyFromAllActions(string keyAlias)
        {
            bool removed = false;
            var keysToRemove = keyToActionCache.Keys.Where(k => k.Item2.Equals(keyAlias, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var (mapName, key) in keysToRemove)
            {
                string actionName = keyToActionCache[(mapName, key)];
                if (UnbindKeyFromAction(keyAlias, actionName))
                    removed = true;
            }

            if (!removed)
                ConsoleLogger.Log($"Key {keyAlias} is not bound to any action");

            return removed;
        }


        public void UnbindAllFromAction(string actionName)
        {
            var action = FindInputAction(actionName);
            if (action == null)
                return;

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
            ConsoleLogger.Log($"Removed all bindings from {actionName} in map {mapName}");
        }

        public string GetActionForKey(string keyAlias, string mapName)
        {
            keyAlias = keyAlias.ToLower();
            return keyToActionCache.TryGetValue((mapName, keyAlias), out string action) ? action : null;
        }

        public List<string> GetActionsForKey(string keyAlias)
        {
            keyAlias = keyAlias.ToLower();

            // Look through all maps
            var actions = keyToActionCache
                .Where(kvp => kvp.Key.key == keyAlias) // ignore mapName
                .Select(kvp => kvp.Value)
                .Distinct() // avoid duplicates if the key appears in multiple maps for the same action
                .ToList();

            return actions;
        }


        public List<string> GetKeysForAction(string actionName)
        {
            var keys = new List<string>();
            if (!actionBindings.TryGetValue(actionName, out var bindings))
                return keys;

            foreach (var binding in bindings)
            {
                if (!string.IsNullOrEmpty(binding.path))
                {
                    string alias = GetKeyAlias(binding.path, true);
                    if (!string.IsNullOrEmpty(alias))
                        keys.Add(alias);
                }
            }

            return keys;
        }


        public Dictionary<string, List<string>> GetAllBindings()
        {
            var allBindings = new Dictionary<string, List<string>>();

            foreach (var kvp in actionBindings)
            {
                var keys = GetKeysForAction(kvp.Key);
                if (keys.Count > 0)
                    allBindings[kvp.Key] = keys;
            }

            return allBindings;
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
    }
}