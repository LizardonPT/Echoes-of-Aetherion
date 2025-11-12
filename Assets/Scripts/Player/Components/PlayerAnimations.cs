using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerAnimations : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [field: SerializeField] public bool LookAtPointer { get; private set; } = true;

        private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
        private static readonly int XHash = Animator.StringToHash("x");
        private static readonly int YHash = Animator.StringToHash("y");

        private void Awake()
        {
            OnValidate();
        }

        private void Update()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                anim.SetTrigger("Hurt");
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                anim.SetTrigger("Death");
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                anim.SetTrigger("Heal");
            }
        }

        public void UpdateAnimation(Vector2 movementInput, Vector2 lookDirection)
        {
            bool isMoving = movementInput.magnitude > 1e-5f;

            anim.SetBool(IsMovingHash, isMoving);

            if (LookAtPointer)
            {
                lookDirection.Normalize();

                anim.SetFloat(XHash, lookDirection.x);
                anim.SetFloat(YHash, lookDirection.y);
            }
            else if (isMoving)
            {
                movementInput.Normalize();
                anim.SetFloat(XHash, movementInput.x);
                anim.SetFloat(YHash, movementInput.y);
            }
        }

        private void OnValidate()
        {
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }
    }
}