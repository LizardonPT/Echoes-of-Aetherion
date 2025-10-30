using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Game.Audio
{
    public class SceneAudioPlayer : MonoBehaviour
    {
        [Header("Ambient Sounds")]
        public List<EventReference> ambientSounds;
        private readonly List<FMOD.Studio.EventInstance> ambientInstances = new();

        [Header("Scene Music")]
        public EventReference music;
        private FMOD.Studio.EventInstance musicInstance;

        [Header("Settings")]
        public bool playOnStart = true;

        private void Start()
        {
            if (playOnStart)
            {
                PlayAmbientSounds();
                PlayMusic();
            }
        }

        private void OnDestroy()
        {
            StopAllSounds();
        }

        public void PlayAmbientSounds()
        {
            StopAmbientSounds();

            foreach (var sound in ambientSounds)
            {
                var instance = RuntimeManager.CreateInstance(sound);
                instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                instance.start();
                ambientInstances.Add(instance);
            }
        }

        public void PlayMusic()
        {
            StopMusic();

            if (music.IsNull) return;

            musicInstance = RuntimeManager.CreateInstance(music);
            musicInstance.start();
        }

        public void StopAmbientSounds()
        {
            foreach (var instance in ambientInstances)
            {
                instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                instance.release();
            }
            ambientInstances.Clear();
        }

        public void StopMusic()
        {
            if (musicInstance.isValid())
            {
                musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                musicInstance.release();
                musicInstance.clearHandle();
            }
        }

        public void StopAllSounds()
        {
            StopAmbientSounds();
            StopMusic();
        }
    }
}
