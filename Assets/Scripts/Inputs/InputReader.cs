using EchoesOfAetherion.Extentions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfAetherion.Inputs
{
    public class InputReader : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveInputReference;
        [SerializeField] private InputActionReference pauseInputReference;

        public Vector2 MovementInput
        {
            get
            {
                if (moveInputReference?.action?.TryReadValue(out Vector2 value) ?? false)
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
                return pauseInputReference?.action?.WasPressedThisFrame() ?? false;
            }
        }
    }
}