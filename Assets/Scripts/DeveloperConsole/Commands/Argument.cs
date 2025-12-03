using System;
using System.Globalization;

namespace EchoesOfEtherion.DeveloperConsole.Commands
{
    [Serializable]
    public struct Argument
    {
        public string Value { get; private set; }

        public Argument(string value)
        {
            Value = value;
        }

        public readonly string GetString() => Value;

        public readonly bool TryGetNumber(out float result)
        {
            var styles = NumberStyles.AllowThousands |
                         NumberStyles.AllowDecimalPoint |
                         NumberStyles.Float;

            string normalized = Value.StartsWith(".") ? "0" + Value : Value;

            return float.TryParse(
                normalized,
                styles,
                CultureInfo.InvariantCulture,
                out result
            );

        }

        public readonly bool TryGetBoolean(out bool result)
        {
            // Check by hand common boolean representations

            if (Value == "0" || Value == "1")
            {
                result = Value == "1"; // if not 1, it's 0.
                return true;
            }

            if (Value == "enable" || Value == "disable")
            {
                result = Value == "enable"; // if not enable, it's disable.
                return true;
            }

            if (Value == "y" || Value == "n")
            {
                result = Value == "y";
                return true;
            }

            if (Value == "yes" || Value == "no")
            {
                result = Value == "yes";
                return true;
            }

            // if any didn't match, fallback to bool.TryParse (true/false)
            return bool.TryParse(Value, out result);
        }

        public override readonly string ToString()
        {
            return Value?.ToString() ?? "null";
        }
    }
}