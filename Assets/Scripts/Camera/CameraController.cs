using System.Collections;
using System.Collections.Generic;
using EchoesOfAetherion.ScriptableObjects.Channels;
using JetBrains.Annotations;
using UnityEngine;

namespace EchoesOfAetherion.CameraUtils
{
    [RequireComponent(typeof(Camera), typeof(CameraFollow), typeof(CameraShaker))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CameraChannel channel;

        private Camera mainCamera;
        private CameraFollow cameraFollow;
        private CameraShaker cameraShaker;

        private void Awake()
        {
            mainCamera ??= GetComponent<Camera>();
            cameraFollow ??= GetComponent<CameraFollow>();
            cameraShaker ??= GetComponent<CameraShaker>();

            channel.RegisterCamera(mainCamera, cameraFollow, cameraShaker);
        }
    }
}
