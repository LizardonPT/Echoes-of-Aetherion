using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfAetherion.Game
{
    [RequireComponent(typeof(TickRegistration))]
    public class TickController : MonoBehaviour
    {
        private readonly List<ITickable> tickables = new();

        public bool Paused { get; private set; } = false;

        private void Awake()
        {
            TickRegistration.OnTickableRegistered += Register;
            TickRegistration.OnTickableUnregistered += UnRegister;
        }
        
        public void Register(ITickable tickable)
        {
            if (!tickables.Contains(tickable))
                tickables.Add(tickable);
        }

        public void UnRegister(ITickable tickable)
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
