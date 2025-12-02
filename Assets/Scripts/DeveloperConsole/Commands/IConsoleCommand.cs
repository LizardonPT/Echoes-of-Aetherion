using System;
using System.Collections.Generic;

namespace EchoesOfEtherion.DeveloperConsole.Commands
{
    public interface IConsoleCommand
    {
        string Key { get; }
        string Description { get; }
        string Usage { get; }
        bool Execute(List<Argument> arguments);
    }
}
