using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerAnimations : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [field: SerializeField] public bool LookAtPointer { get; private set; } = true;

        private PlayerSpellCaster spellCaster;
        private HealthSystem healthSystem;

        private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
        private static readonly int XHash = Animator.StringToHash("x");
        private static readonly int YHash = Animator.StringToHash("y");

        private void Awake()
        {
            OnValidate();
            spellCaster = GetComponent<PlayerSpellCaster>();
            healthSystem = GetComponent<HealthSystem>();
        }

        private void OnEnable()
        {
            spellCaster.SpellCasted += OnSpellCasted;
            healthSystem.Damaged += OnDamaged;
            healthSystem.Healed += OnHealed;
        }

        private void OnDisable()
        {
            spellCaster.SpellCasted -= OnSpellCasted;
            healthSystem.Damaged -= OnDamaged;
            healthSystem.Healed -= OnHealed;
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

        private void OnSpellCasted()
        {
            anim.SetTrigger("Attack");
        }

        private void OnDamaged(float damageAmount)
        {
            anim.SetTrigger("Hurt");
        }

        private void OnHealed(float healAmount)
        {
            anim.SetTrigger("Heal");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }
    }
#endif
}