using System;
using System.Collections.Generic;
using EchoesOfEtherion.CameraUtils;
using EchoesOfEtherion.Game;
using UnityEngine;

namespace EchoesOfEtherion.ScriptableObjects.Channels
{
    [CreateAssetMenu(fileName = "CameraChannel", menuName = "Scriptable Objects/Channels/CameraChannel")]
    public class CameraChannel : ScriptableObject
    {
        public Camera MainCamera { get; private set; }
        public CameraFollow CameraFollow { get; private set; }
        public CameraShaker CameraShaker { get; private set; }

        public void RegisterCamera(Camera camera, CameraFollow cameraFollow, CameraShaker shaker)
        {
            MainCamera = camera;
            CameraFollow = cameraFollow;
            CameraShaker = CameraShaker;
        }
    }
}
