using EchoesOfEtherion.CameraUtils;
using EchoesOfEtherion.HealthSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerAnimations : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private float updateLookDirectionMinDistance = 8;
        [field: SerializeField] public bool LookAtPointer { get; private set; } = true;

        private PlayerSpellCaster spellCaster;

        private static readonly int isMovingHash = Animator.StringToHash("isMoving");
        private static readonly int xHash = Animator.StringToHash("x");
        private static readonly int yHash = Animator.StringToHash("y");
        private static readonly int attackHash = Animator.StringToHash("Attack");
        private static readonly int hurtHash = Animator.StringToHash("Hurt");
        private static readonly int healHash = Animator.StringToHash("Heal");
        private static readonly int dieHash = Animator.StringToHash("Die");

        private Vector2 lastLookDistance;

        private void Awake()
        {
            spellCaster = GetComponent<PlayerSpellCaster>();
        }

        private void OnEnable()
        {
            spellCaster.SpellCasted += OnSpellCasted;
        }

        private void OnDisable()
        {
            spellCaster.SpellCasted -= OnSpellCasted;
        }

        public void UpdateWalkAnimation(Vector2 movementInput, Vector2 lookDirection)
        {
            bool isMoving = movementInput.magnitude > 1e-5f;

            anim.SetBool(isMovingHash, isMoving);

            if (LookAtPointer)
            {
                Vector2 lookDir = Vector2.zero;

                if (Mouse.current == null || CameraController.Instance.GameCamera == null)
                {
                    if (movementInput != Vector2.zero)
                        lookDir = movementInput.normalized;
                }
                else
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    Vector3 worldPos = CameraController.Instance.GameCamera.ScreenToWorldPoint(
                        new Vector3(mousePos.x, mousePos.y, CameraController.Instance.GameCamera.nearClipPlane)
                    );

                    if (Vector2.Distance(worldPos, transform.position) > updateLookDirectionMinDistance)
                    {
                        lookDir = lookDirection.normalized;
                        lastLookDistance = lookDir;
                    }
                    else
                    {
                        lookDir = lastLookDistance;
                    }
                }

                anim.SetFloat(xHash, lookDir.x);
                anim.SetFloat(yHash, lookDir.y);
            }
            else if (isMoving)
            {
                movementInput.Normalize();
                anim.SetFloat(xHash, movementInput.x);
                anim.SetFloat(yHash, movementInput.y);
            }
        }

        private void OnSpellCasted()
        {
            anim.SetTrigger(attackHash);
        }

        public void Damage()
        {
            anim.SetTrigger(hurtHash);
        }

        public void Heal()
        {
            anim.SetTrigger(healHash);
        }

        public void Die()
        {
            anim.SetTrigger(dieHash);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }
#endif
    }
}