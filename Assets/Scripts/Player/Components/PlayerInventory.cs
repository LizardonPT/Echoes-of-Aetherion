using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using EchoesOfEtherion.Spells;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;

        public Dictionary<int, Spell> Slots { get; private set; }

        private List<Spell> spells;

        private void Awake()
        {
            Slots = new Dictionary<int, Spell>
            {
                {1, null},
                {2, null},
                {3, null},
                {4, null},
                {5, null},
                {6, null},
            };
            spells = new List<Spell>();
        }

        public void AddSpellPage(Spell page)
        {
            if (!spells.Contains(page))
            {
                spells.Add(page);
                for (int i = 1; i <= Slots.Count; i++)
                {
                    if (Slots[i] == null)
                    {
                        Slots[i] = page;
                        break;
                    }
                }
                Log($"Page {page.SpellName} was added to the inventory.");
                if (enableLogging)
                {
                    string spells = "";
                    for (int i = 0; i < this.spells.Count; i++)
                    {
                        spells += this.spells[i].SpellName;
                        spells += i < this.spells.Count - 1 ? ", " : ".";
                    }
                    Log($"Inventory is now: {spells}");
                }
            }
        }
        public void RemoveSpellPage(Spell page)
        {
            if (spells.Contains(page)) spells.Remove(page);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out CollectableSpell collectableSpell))
            {
                Log($"Trigger page {collectableSpell.Spell.SpellName}");
                AddSpellPage(collectableSpell.Spell);
                Destroy(collectableSpell.gameObject);
            }
        }

        public Spell GetSpellInSlot(int i)
        {
            if (i >= 0 && i < Slots.Count)
            {
                return Slots[i];
            }
            else return null;
        }

        private void Log(string message)
        {
            if (enableLogging)
                Debug.Log($"[PlayerInventory] {message}");
        }
    }
}
