using System;
using EchoesOfEtherion.Spells;
using UnityEngine;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerSpellCaster : MonoBehaviour
    {
        [SerializeField] public Transform CasterPos;
        private PlayerInventory inventory;
        private PlayerAnimations animator;

        public event Action SpellCasted;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
            animator = GetComponent<PlayerAnimations>();
        }

        private void OnEnable()
        {
            inventory.SlotPressed += CastSpellInSlot;
        }

        private void OnDisable()
        {
            inventory.SlotPressed -= CastSpellInSlot;
        }

        public void CastSelectedSpell()
        {
            Spell spell = inventory.SelectedSpell;
            if (spell == null)
                return;

            if (spell.ExecuteSpell(this))
                animator.PlayAnimationByName(spell.AnimationName);

            SpellCasted?.Invoke();
        }

        private void Update()
        {
            foreach (Spell spell in inventory.Slots.Values)
            {
                if (spell != null)
                    spell.UpdateSpell(this);
            }
        }

        private void CastSpellInSlot(int slot)
        {
            Spell spell = inventory.GetSpellInSlot(slot);
            if (spell == null) return;

            if (spell.ExecuteSpell(this))
                animator.PlayAnimationByName(spell.AnimationName);

            SpellCasted?.Invoke();
        }
    }
}