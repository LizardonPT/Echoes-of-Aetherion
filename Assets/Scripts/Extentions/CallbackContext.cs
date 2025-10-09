using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace EchoesOfAetherion.Extentions
{
    public static class CallbackContextExtension
    {
        public static bool TryReadValue<T>(this CallbackContext context, out T value) where T : struct
        {
            value = default;
            try
            {
                value = context.ReadValue<T>();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
