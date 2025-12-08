using System;
using System.Collections.Generic;

namespace EchoesOfEtherion.DeveloperConsole.Commands
{
    public class SettingCommand : ConsoleCommand
    {
        public bool IsPersistent { get; private set; } = true;
        public string Category { get; private set; } = "General";
        public Func<string> Getter { get; private set; }
        private readonly Action<Argument> setter;

        public SettingCommand(string key, string description, string usage,
                             Action<Argument> setter, Func<string> getter,
                             bool isPersistent = false, string category = "General")
        {
            Key = key;
            Description = description;
            Usage = usage;
            IsPersistent = isPersistent;
            Category = category;
            Getter = getter;
            this.setter = setter;
            
            expectedArguments = new() { new() };
        }

        public override bool Execute(List<Argument> arguments)
        {
            if (arguments.Count > 0)
            {
                setter?.Invoke(arguments[0]);
                return true;
            }

            return false;
        }
    }
}