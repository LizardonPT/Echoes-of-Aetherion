using System;
using System.Collections.Generic;
using EchoesOfAetherion.CameraUtils;
using EchoesOfAetherion.Game;
using UnityEngine;

namespace EchoesOfAetherion.ScriptableObjects.Channels
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
