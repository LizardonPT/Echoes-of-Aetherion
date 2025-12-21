using System;
using EchoesOfEtherion.Extentions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfEtherion.Game.Utils
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/Utils/InputReader")]
    public class InputReader : ScriptableObject
    {
        [Header("Movement")]
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

        [Header("Interaction")]
        [SerializeField]
        private InputActionReference interactActionReference;

        [Header("Essential Spells")]
        [SerializeField]
        private InputActionReference healActionReference;
        [SerializeField]
        private InputActionReference basicProjectileActionReference;
        [SerializeField]
        private InputActionReference blinkActionReference;

        [Header("Slot Spells (Set 1-4)")]
        [SerializeField]
        private InputActionReference slot1ActionReference;
        [SerializeField]
        private InputActionReference slot2ActionReference;
        [SerializeField]
        private InputActionReference slot3ActionReference;
        [SerializeField]
        private InputActionReference slot4ActionReference;

        [Header("Spell Set Management")]
        [SerializeField]
        private InputActionReference nextSpellSetActionReference;
        [SerializeField]
        private InputActionReference previousSpellSetActionReference;

        [Header("Legacy (for backward compatibility)")]
        [SerializeField]
        private InputActionReference slot5ActionReference;
        [SerializeField]
        private InputActionReference slot6ActionReference;
        [SerializeField]
        private InputActionReference shootSpellActionReference;

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

        public bool PauseInputPressed => pauseActionReference?.action?.WasPressedThisFrame() ?? false;
        public bool InteractInputPressed => interactActionReference?.action?.WasPressedThisFrame() ?? false;

        public bool HealInputPressed => healActionReference?.action?.WasPressedThisFrame() ?? false;
        public bool BasicProjectileInputPressed => basicProjectileActionReference?.action?.WasPressedThisFrame() ?? false;
        public bool BlinkInputPressed => blinkActionReference?.action?.WasPressedThisFrame() ?? false;

        public bool SpellSlot1InputPressed => slot1ActionReference?.action?.WasPressedThisFrame() ?? false;
        public bool SpellSlot2InputPressed => slot2ActionReference?.action?.WasPressedThisFrame() ?? false;
        public bool SpellSlot3InputPressed => slot3ActionReference?.action?.WasPressedThisFrame() ?? false;
        public bool SpellSlot4InputPressed => slot4ActionReference?.action?.WasPressedThisFrame() ?? false;

        public bool NextSpellSetInputPressed => nextSpellSetActionReference?.action?.WasPressedThisFrame() ?? false;
        public bool PreviousSpellSetInputPressed => previousSpellSetActionReference?.action?.WasPressedThisFrame() ?? false;

        public void EnableAllActions()
        {
            EnableAction(moveUpActionReference);
            EnableAction(moveDownActionReference);
            EnableAction(moveLeftActionReference);
            EnableAction(moveRightActionReference);
            EnableAction(pauseActionReference);
            EnableAction(interactActionReference);
            EnableAction(healActionReference);
            EnableAction(basicProjectileActionReference);
            EnableAction(blinkActionReference);
            EnableAction(slot1ActionReference);
            EnableAction(slot2ActionReference);
            EnableAction(slot3ActionReference);
            EnableAction(slot4ActionReference);
            EnableAction(nextSpellSetActionReference);
            EnableAction(previousSpellSetActionReference);
            EnableAction(slot5ActionReference);
            EnableAction(slot6ActionReference);
            EnableAction(shootSpellActionReference);
        }

        public void DisableAllActions()
        {
            DisableAction(moveUpActionReference);
            DisableAction(moveDownActionReference);
            DisableAction(moveLeftActionReference);
            DisableAction(moveRightActionReference);
            DisableAction(pauseActionReference);
            DisableAction(interactActionReference);
            DisableAction(healActionReference);
            DisableAction(basicProjectileActionReference);
            DisableAction(blinkActionReference);
            DisableAction(slot1ActionReference);
            DisableAction(slot2ActionReference);
            DisableAction(slot3ActionReference);
            DisableAction(slot4ActionReference);
            DisableAction(nextSpellSetActionReference);
            DisableAction(previousSpellSetActionReference);
            DisableAction(slot5ActionReference);
            DisableAction(slot6ActionReference);
            DisableAction(shootSpellActionReference);
        }

        private void EnableAction(InputActionReference actionReference)
        {
            actionReference?.action?.Enable();
        }

        private void DisableAction(InputActionReference actionReference)
        {
            actionReference?.action?.Disable();
        }

        private void OnEnable()
        {
            EnableAllActions();
        }

        private void OnDisable()
        {
            DisableAllActions();
        }
    }
}
