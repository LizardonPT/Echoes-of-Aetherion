using UnityEngine;
using System.Collections.Generic;

namespace EchoesOfEtherion.Game
{
    public class GamePauser : MonoBehaviour
    {
        private readonly List<Animator> pausedAnimators = new();
        private readonly List<Rigidbody2D> pausedRigidbodies = new();

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
                    pausedRigidbodies.Add(rb);
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
            for (int i = 0; i < pausedRigidbodies.Count; i++)
            {
                Rigidbody2D rb = pausedRigidbodies[i];
                if (rb != null)
                    rb.WakeUp();
            }
            pausedRigidbodies.Clear();
        }
    }
}
