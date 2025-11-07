using System;
using EchoesOfEtherion.Extentions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfEtherion.ScriptableObjects.Utils
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/Utils/InputReader")]
    public class InputReader : ScriptableObject
    {
        [SerializeField]
        private InputActionReference moveActionReference;
        [SerializeField]
        private InputActionReference pauseActionReference;

        [SerializeField]
        private InputActionReference interactActionReference;

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

        public bool InteractInputPressed
        {
            get
            {
                return interactActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }
    }
}
