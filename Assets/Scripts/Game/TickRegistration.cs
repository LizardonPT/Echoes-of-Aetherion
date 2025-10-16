using System;
using UnityEngine;

namespace EchoesOfAetherion.Game
{
    public class TickRegistration : MonoBehaviour
    {
        public static event Action<ITickable> OnTickableRegistered;
        public static event Action<ITickable> OnTickableUnregistered;

        public static void Register(ITickable tickable)
        {
            OnTickableRegistered?.Invoke(tickable);
        }

        public static void UnRegister(ITickable tickable)
        {
            OnTickableUnregistered?.Invoke(tickable);
        }
    }
}
