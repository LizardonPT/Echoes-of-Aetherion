using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfAetherion.Input
{
    public class InputReader : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveAction;

        public Vector2 MovementInput { get; private set; }

        private void OnEnable()
        {
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
        }

        private void OnDisable()
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }
    }
}