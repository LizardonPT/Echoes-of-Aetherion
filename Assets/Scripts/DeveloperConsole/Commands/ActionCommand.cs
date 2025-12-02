using System;
using System.Collections.Generic;

namespace EchoesOfEtherion.DeveloperConsole.Commands
{
    public class ActionCommand : ConsoleCommand
    {
        private readonly Action<List<Argument>> action;

        public ActionCommand(string key, string description, string usage,
                            Action<List<Argument>> action, List<Argument> expectedArgs = null)
        {
            Key = key;
            Description = description;
            Usage = usage;
            this.action = action;

            if (expectedArgs != null)
                expectedArguments = expectedArgs;
        }

        public override bool Execute(List<Argument> arguments)
        {
            try
            {
                action?.Invoke(arguments);
                return true;
            }
            catch (Exception e)
            {
                ConsoleLogger.Log($"Error executing command '{Key}': {e.Message}");
                return false;
            }
        }
    }
}