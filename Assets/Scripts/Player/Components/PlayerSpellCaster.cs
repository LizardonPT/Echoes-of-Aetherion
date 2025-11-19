using System;
using EchoesOfEtherion.Spells;
using UnityEngine;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerSpellCaster : MonoBehaviour
    {
        [SerializeField] private Transform casterPos;
        private PlayerInventory inventory;
        private PlayerAnimations animator;

        public event Action SpellCasted;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
            animator = GetComponent<PlayerAnimations>();
        }

        public void CastSpell(int slot)
        {
            Spell spell = inventory.GetSpellInSlot(slot);
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
    }
}