using System;
using System.Collections.Generic;
using EchoesOfEtherion.CameraUtils;
using EchoesOfEtherion.Game;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.ScriptableObjects.Channels
{
    [CreateAssetMenu(fileName = "AudioChannel", menuName = "Scriptable Objects/Channels/AudioChannel")]
    public class AudioChannel : ScriptableObject
    {
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }
    }
}
