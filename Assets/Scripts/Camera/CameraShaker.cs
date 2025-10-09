using UnityEngine;
using System.Collections;

namespace EchoesOfAetherion.CameraUtils
{
    public class CameraShaker : MonoBehaviour
    {
        private Vector3 originalPosition;
        private bool isShaking = false;

        private void Awake()
        {
            originalPosition = transform.localPosition;
        }

        /// <summary>
        /// Shake the camera with given parameters.
        /// </summary>
        /// <param name="frequency">How often the camera position changes per second.</param>
        /// <param name="amplitude">How far the camera can move from its original position in each direction.</param>
        /// <param name="duration">Duration of the shake in seconds.</param>
        public void Shake(float frequency, float amplitude, float duration)
        {
            if (!isShaking)
            {
                StartCoroutine(DoCameraShake(frequency, amplitude, duration));
            }
        }

        private IEnumerator DoCameraShake(float frequency, float amplitude, float duration)
        {
            isShaking = true;

            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                // Calculate random offsets for x and y directions using Perlin noise
                float randomX = Mathf.PerlinNoise(Time.time * frequency, 0f) * 2f - 1f;
                float randomY = Mathf.PerlinNoise(0f, Time.time * frequency) * 2f - 1f;

                // Apply the calculated shake to the camera's position
                transform.localPosition = originalPosition + new Vector3(randomX, randomY, 0f) * amplitude;

                elapsed += Time.deltaTime;

                yield return null;
            }

            // Reset the camera position to its original position after the shake ends
            isShaking = false;
            transform.localPosition = originalPosition;
        }
    }
}
