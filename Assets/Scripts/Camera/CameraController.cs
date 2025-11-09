using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace EchoesOfEtherion.CameraUtils
{
    [RequireComponent(typeof(Camera), typeof(CameraFollow), typeof(CameraShaker))]
    public class CameraController : Singleton<CameraController>
    {
        public Camera GameCamera;
        public CameraFollow CameraFollow;
        public CameraShaker CameraShaker;

        protected override void Awake()
        {
            base.Awake();
            GameCamera ??= GetComponent<Camera>();
            CameraFollow ??= GetComponent<CameraFollow>();
            CameraShaker ??= GetComponent<CameraShaker>();
        }
    }
}
