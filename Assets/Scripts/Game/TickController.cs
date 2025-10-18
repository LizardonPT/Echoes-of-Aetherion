using System.Collections.Generic;
using EchoesOfAetherion.ScriptableObjects.Channels;
using UnityEngine;

namespace EchoesOfAetherion.Game
{
    public class TickController : MonoBehaviour
    {
        [SerializeField]
        private TickChannel tickChannel;

        public bool Paused { get; private set; } = false;

        private void Update()
        {
            if (Paused) return;
            tickChannel.UpdateTickables();
        }

        private void FixedUpdate()
        {
            if (Paused) return;
            tickChannel.FixedUpdateTickables();
        }

        public void SetPaused(bool value)
        {
            Paused = value;
        }
    }
}
