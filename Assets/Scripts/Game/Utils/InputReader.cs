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
        private InputActionReference moveUpActionReference;
        [SerializeField]
        private InputActionReference moveDownActionReference;
        [SerializeField]
        private InputActionReference moveLeftActionReference;
        [SerializeField]
        private InputActionReference moveRightActionReference;
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
        [SerializeField]
        private InputActionReference shootSpellActionReference;

        public bool AttackInputPressed
        {
            get
            {
                return shootSpellActionReference?.action?.WasPressedThisFrame() ?? false;
            }
        }

        public Vector2 MovementInput
        {
            get
            {
                float x = 0f;
                float y = 0f;

                if (moveRightActionReference?.action != null && moveRightActionReference.action.IsPressed())
                {
                    x += 1f;
                }
                if (moveLeftActionReference?.action != null && moveLeftActionReference.action.IsPressed())
                {
                    x -= 1f;
                }
                if (moveUpActionReference?.action != null && moveUpActionReference.action.IsPressed())
                {
                    y += 1f;
                }
                if (moveDownActionReference?.action != null && moveDownActionReference.action.IsPressed())
                {
                    y -= 1f;
                }

                return new Vector2(x, y).normalized;
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
