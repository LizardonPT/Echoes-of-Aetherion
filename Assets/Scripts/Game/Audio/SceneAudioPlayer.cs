using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace EchoesOfEtherion.Game.Audio
{
    public class WindAudioPlayer : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private EventReference windEvent;
        [SerializeField] private string panningParameterName = "Pan";

        [Header("Settings")]
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private float panningFrequency = 1;
        [SerializeField] private float panningAmplitude = 1;

        private bool isPlaying = false;
        private EventInstance windInstance;

        private void Start()
        {
            if (playOnStart)
            {
                PlayWindSound();
            }
        }

        private void FixedUpdate()
        {
            if (isPlaying)
            {
                float noise = Mathf.PerlinNoise(Time.time * panningFrequency, 1.0f);
                float panningValue = ((noise * 2f) - 1f) * panningAmplitude;
                windInstance.setParameterByName(panningParameterName, panningValue);
            }
        }

        [Button("Play Wind Sound")]
        public void PlayWindSound()
        {
            if (!isPlaying)
            {
                windInstance = RuntimeManager.CreateInstance(windEvent);
                windInstance.start();
                isPlaying = true;
            }
        }

        [Button("Stop Wind Sound")]
        public void StopWindSound()
        {
            if (isPlaying)
            {
                windInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                windInstance.release();

                isPlaying = false;
            }
        }

        //! Unsure the sound stops and is cleaned.
        private void OnDestroy()
        {
            StopWindSound();
        }
    }
}
