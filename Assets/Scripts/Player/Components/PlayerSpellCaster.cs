using System;
using EchoesOfEtherion.Spells;
using UnityEngine;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerSpellCaster : MonoBehaviour
    {
        [SerializeField] private Transform casterPos;
        private PlayerInventory inventory;

        public event Action SpellCasted;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
        }

        public void CastSpell(int slot, Vector2 direction)
        {
            SpellPage page = inventory.GetSpellInSlot(slot);
            if (page == null)
                return;

            LightBallSpell spellInstance = Instantiate(page.SpellPrefab, casterPos.position, Quaternion.identity, casterPos)
                                            .GetComponent<LightBallSpell>();

            spellInstance.ExecuteSpell(casterPos.position, direction);
            SpellCasted?.Invoke();
        }
    }
}