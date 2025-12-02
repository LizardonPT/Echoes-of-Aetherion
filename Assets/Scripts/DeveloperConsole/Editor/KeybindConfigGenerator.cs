#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace EchoesOfEtherion.DeveloperConsole.Editor
{
    public static class KeybindConfigGenerator
    {
        public const string cfgFileName = "default_binds.cfg";

        [MenuItem("Tools/Developer Console/Generate Default Keybinds.cfg")]
        public static void GenerateDefaultKeybinds()
        {
            // Locate Input Action Asset
            string assetPath = "Assets/Settings/InputSystem_Actions.inputactions";
            InputActionAsset inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(assetPath);

            if (inputAsset == null)
            {
                Debug.LogError($"Could not find Input Action Asset at path: {assetPath}");
                return;
            }

            //  Build the CFG file content
            StringBuilder cfgContent = new();
            cfgContent.AppendLine($"// Generated from: {inputAsset.name}");
            cfgContent.AppendLine($"// Generated on: {System.DateTime.Now}");
            cfgContent.AppendLine($"// DO NOT EDIT - Auto-generated file");
            cfgContent.AppendLine();

            // Process each action map
            foreach (InputActionMap actionMap in inputAsset.actionMaps)
            {
                cfgContent.AppendLine($"// === {actionMap.name.ToUpper()} ===");

                foreach (InputAction action in actionMap.actions)
                {
                    if (action.bindings.Count == 0)
                        continue;

                    ProcessSimpleAction(cfgContent, action);
                }

                cfgContent.AppendLine();
            }

            // Write to file
            string configsFolder = "cfg";
            string configsPath = Path.Combine(Application.streamingAssetsPath, configsFolder);

            // Create directory if it doesn't exist
            if (!Directory.Exists(configsPath))
            {
                Directory.CreateDirectory(configsPath);
                Debug.Log($"Created configs directory: {configsPath}");
            }

            string outputPath = Path.Combine(configsPath, cfgFileName);

            if (!File.Exists(outputPath))
            {
                File.Create(outputPath).Dispose();
            }

            // Remove read-only
            var originalAttributes = File.GetAttributes(outputPath);
            File.SetAttributes(outputPath, originalAttributes & ~FileAttributes.ReadOnly);

            try
            {
                File.WriteAllText(outputPath, cfgContent.ToString());
            }
            finally
            {
                // Restore read-only
                File.SetAttributes(outputPath, originalAttributes | FileAttributes.ReadOnly);
            }


            Debug.Log($"Successfully generated default keybinds at: {outputPath}");

            // Open the file location in explorer
            EditorUtility.RevealInFinder(outputPath);
        }

        private static void ProcessSimpleAction(StringBuilder cfgContent, InputAction action)
        {
            foreach (InputBinding binding in action.bindings)
            {
                // Skip composite parts and empty paths
                if (binding.isPartOfComposite || binding.isComposite ||
                    string.IsNullOrEmpty(binding.path))
                    continue;

                string keyAlias = GetKeyAlias(binding.path);

                if (!string.IsNullOrEmpty(keyAlias))
                {
                    string commandName = action.name.ToLower();
                    cfgContent.AppendLine($"bind {keyAlias} {commandName}");
                }
            }
        }

        private static string GetKeyAlias(string inputPath)
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

            // Keyboard keys
            if (inputPath.StartsWith("<keyboard>/"))
            {
                string key = inputPath.Substring(11); // remove "<Keyboard>/"

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

            // Return as-is if not recognized
            return inputPath;
        }
    }
}
#endif