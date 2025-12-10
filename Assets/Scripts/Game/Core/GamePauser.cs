using UnityEngine;
using System.Collections.Generic;

namespace EchoesOfEtherion.Game.Core
{
    public class GamePauser : MonoBehaviour
    {
        private readonly List<Animator> pausedAnimators = new();
        private readonly Dictionary<Rigidbody2D, bool> pausedRigidbodies = new();

        public void PauseGame()
        {
            // Pause all animators
            var animators = FindObjectsByType<Animator>(FindObjectsSortMode.None);
            foreach (var animator in animators)
            {
                if (animator.enabled)
                {
                    pausedAnimators.Add(animator);
                    animator.enabled = false;
                }
            }

            // Pause all rigidbodies
            var rigidBodies = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);
            foreach (var rb in rigidBodies)
            {
                if (!rb.IsSleeping())
                {
                    pausedRigidbodies.Add(rb, rb.IsSleeping());
                    rb.Sleep();
                }
            }
        }

        public void ResumeGame()
        {
            // Resume all animators
            foreach (var animator in pausedAnimators)
            {
                if (animator != null)
                    animator.enabled = true;
            }
            pausedAnimators.Clear();

            // Resume all rigidbodies
            foreach (Rigidbody2D rb in pausedRigidbodies.Keys)
            {
                if (rb == null) continue;
                if (pausedRigidbodies.TryGetValue(rb, out bool b))
                {
                    if (!b)
                        rb.WakeUp();
                }
            }
            pausedRigidbodies.Clear();
        }
    }
}
