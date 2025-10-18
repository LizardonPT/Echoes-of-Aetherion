using System;
using EchoesOfAetherion.Extentions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfAetherion.ScriptableObjects.Utils
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/Utils/InputReader")]
    public class InputReader : ScriptableObject
    {
        [SerializeField]
        private InputActionReference moveActionReference;
        [SerializeField]
        private InputActionReference pauseActionReference;

        public Vector2 MovementInput
        {
            get
            {
                if (moveActionReference?.action?.TryReadValue(out Vector2 value) ?? false)
                {
                    return value;
                }
                else
                    return Vector2.zero;
            }
        }

        public bool PauseInputPressed
        {
            get
            {
                return pauseActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }
    }
}
