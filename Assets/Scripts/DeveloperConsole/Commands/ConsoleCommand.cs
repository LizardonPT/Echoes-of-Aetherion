using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfEtherion.DeveloperConsole.Commands
{
    public abstract class ConsoleCommand : IConsoleCommand
    {
        public string Key { get; protected set; }
        public string Description { get; protected set; }
        public string Usage { get; protected set; }

        protected List<Argument> expectedArguments = new();

        public List<Argument> ExpectedArguments => new(expectedArguments);


        public abstract bool Execute(List<Argument> arguments);
    }
}