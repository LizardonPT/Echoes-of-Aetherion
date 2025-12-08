using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using EchoesOfEtherion.Spells;
using EchoesOfEtherion.Game.Utils;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;
        [field: Header("Settings")]
        [field: SerializeField] public bool AutoCast { get; set; } = false;
        [SerializeField] private InputReader inputReader;

        public Dictionary<int, Spell> Slots { get; private set; }
        public Spell SelectedSpell { get; private set; }

        public event Action<Dictionary<int, Spell>> SlotsUpdated;
        public event Action<Spell, int> SelectedSpellChanged;

        private List<Spell> spells;
        public event Action<int> SlotPressed;

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
                        SelectedSpell = Slots[i];
                        SlotsUpdated?.Invoke(Slots);
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
            SlotsUpdated?.Invoke(Slots);
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

        public void UpdateInput()
        {
            for (int i = 1; i <= Slots.Count; i++)
            {
                if (Slots[i] == null)
                    return;

                bool slotPressed = i switch
                {
                    1 => inputReader.Slot1InputPressed,
                    2 => inputReader.Slot2InputPressed,
                    3 => inputReader.Slot3InputPressed,
                    4 => inputReader.Slot4InputPressed,
                    5 => inputReader.Slot5InputPressed,
                    6 => inputReader.Slot6InputPressed,
                    _ => false
                };

                if (slotPressed)
                {
                    SelectedSpell = Slots[i];
                    SelectedSpellChanged?.Invoke(SelectedSpell, i);

                    if (AutoCast)
                    {
                        SlotPressed?.Invoke(i);
                    }

                    break;
                }
            }
        }
        private void Log(string message)
        {
            if (enableLogging)
                Debug.Log($"[PlayerInventory] {message}");
        }
    }
}
