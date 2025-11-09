using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfEtherion.Game
{
    public class TickController : Singleton<TickController>
    {
        public bool Paused { get; private set; } = false;
        private IList<ITickable> tickables;

        public void Register(ITickable tickable)
        {
            tickables ??= new List<ITickable>();

            if (!tickables.Contains(tickable))
                tickables.Add(tickable);
        }

        public void UnRegister(ITickable tickable)
        {
            if (tickables == null)
                return;

            if (tickables.Contains(tickable))
                tickables.Remove(tickable);
        }
        private void Update()
        {
            if (Paused) return;
            UpdateTickables();
        }

        private void FixedUpdate()
        {
            if (Paused) return;
            FixedUpdateTickables();
        }

        public void UpdateTickables()
        {
            if (tickables == null)
                return;

            foreach (ITickable tickable in tickables)
                tickable.Tick();
        }

        public void FixedUpdateTickables()
        {
            if (tickables == null)
                return;

            foreach (ITickable tickable in tickables)
                tickable.FixedTick();
        }

        public void SetPaused(bool value)
        {
            Paused = value;
        }
    }
}
