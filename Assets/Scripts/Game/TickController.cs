using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfAetherion.Game
{
    public class TickController : MonoBehaviour
    {
        private readonly List<ITickable> tickables = new();

        public bool Paused { get; private set; } = false;

        public void Register(ITickable tickable)
        {
            if (!tickables.Contains(tickable))
                tickables.Add(tickable);
        }

        public void Unregister(ITickable tickable)
        {
            tickables.Remove(tickable);
        }

        private void Update()
        {
            if (Paused) return;

            foreach (var tickable in tickables)
                tickable.Tick();
        }

        private void FixedUpdate()
        {
            if (Paused) return;

            foreach (var tickable in tickables)
                tickable.FixedTick();
        }

        public void SetPaused(bool value)
        {
            Paused = value;
        }
    }
}
