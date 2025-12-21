using System;
using EchoesOfEtherion.Spells;
using UnityEngine;

namespace EchoesOfEtherion.Player.Components
{
    [RequireComponent(typeof(PlayerSpellInventory))]
    [RequireComponent(typeof(PlayerAnimations))]
    public class PlayerSpellCaster : MonoBehaviour
    {
        [SerializeField] public Transform CasterPos;
        private PlayerSpellInventory inventory;
        private PlayerAnimations animator;

        public event Action SpellCasted;

        private void Awake()
        {
            inventory = GetComponent<PlayerSpellInventory>();
            animator = GetComponent<PlayerAnimations>();
        }

        public void CastSlotSpell(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 4)
            {
                Debug.LogWarning($"Invalid slot index: {slotIndex}");
                return;
            }

            Spell spell = inventory.GetSpellInCurrentSet(slotIndex);
            if (spell == null)
            {
                return;
            }

            if (spell.ExecuteSpell(this))
            {
                animator.PlayAnimationByName(spell.AnimationName);
                SpellCasted?.Invoke();
            }
        }

        public void CastSpell(Spell spell)
        {
            if (spell.ExecuteSpell(this))
            {
                animator.PlayAnimationByName(spell.AnimationName);
                SpellCasted?.Invoke();
            }
        }

        private void Update()
        {
            UpdateSpellCooldowns(inventory.HealSpell);
            UpdateSpellCooldowns(inventory.BasicProjectileSpell);
            UpdateSpellCooldowns(inventory.BlinkSpell);

            foreach (var spellSet in inventory.AllSpellSets)
            {
                foreach (var spell in spellSet.Slots)
                {
                    UpdateSpellCooldowns(spell);
                }
            }
        }

        private void UpdateSpellCooldowns(Spell spell)
        {
            if (spell != null)
                spell.UpdateSpell(this);
        }
    }
}
