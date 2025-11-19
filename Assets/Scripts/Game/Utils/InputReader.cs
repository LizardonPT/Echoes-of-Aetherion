using System;
using EchoesOfEtherion.Extentions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfEtherion.Game.Utils
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

        [SerializeField]
        private InputActionReference slot1ActionReference;
        [SerializeField]
        private InputActionReference slot2ActionReference;
        [SerializeField]
        private InputActionReference slot3ActionReference;
        [SerializeField]
        private InputActionReference slot4ActionReference;
        [SerializeField]
        private InputActionReference slot5ActionReference;
        [SerializeField]
        private InputActionReference slot6ActionReference;
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

        public bool Slot1InputPressed
        {
            get
            {
                return slot1ActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }

        public bool Slot2InputPressed
        {
            get
            {
                return slot2ActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }
        public bool Slot3InputPressed
        {
            get
            {
                return slot3ActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }
        public bool Slot4InputPressed
        {
            get
            {
                return slot4ActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }
        public bool Slot5InputPressed
        {
            get
            {
                return slot5ActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }
        public bool Slot6InputPressed
        {
            get
            {
                return slot6ActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }
    }
}
