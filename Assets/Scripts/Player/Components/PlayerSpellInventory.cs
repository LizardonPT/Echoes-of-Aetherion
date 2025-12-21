// PlayerSpellInventory.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using EchoesOfEtherion.Spells;

namespace EchoesOfEtherion.Player.Components
{

    public class PlayerSpellInventory : MonoBehaviour
    {
        [Header("Essential Spells")]
        [SerializeField] private Spell healSpell;
        [SerializeField] private Spell basicProjectileSpell;
        [SerializeField] private Spell blinkSpell;

        [Header("Spell Sets")]
        [SerializeField] private List<SpellSet> spellSets = new List<SpellSet>();
        [SerializeField] private int currentSetIndex = 0;

        [Header("Debug")]
        [SerializeField] private bool enableLogging = true;

        public Spell HealSpell => healSpell;
        public Spell BasicProjectileSpell => basicProjectileSpell;
        public Spell BlinkSpell => blinkSpell;

        public SpellSet CurrentSpellSet => spellSets.Count > 0 ? spellSets[currentSetIndex] : null;
        public int CurrentSetIndex => currentSetIndex;
        public int SpellSetCount => spellSets.Count;
        public List<SpellSet> AllSpellSets => spellSets;

        public event Action<SpellSet> OnCurrentSetChanged;
        public event Action<List<SpellSet>> OnSpellSetsUpdated;

        private void Awake()
        {
            if (spellSets.Count == 0)
            {
                spellSets.Add(new SpellSet());
            }
        }

        private void Start()
        {
            OnCurrentSetChanged?.Invoke(CurrentSpellSet);
        }

        public bool TryAddSpellToCurrentSet(Spell spell)
        {
            if (spellSets.Count == 0) return false;

            var currentSet = spellSets[currentSetIndex];

            for (int i = 0; i < currentSet.Slots.Length; i++)
            {
                if (currentSet.Slots[i] == null)
                {
                    currentSet.Slots[i] = spell;
                    Log($"Added {spell} to slot {i + 1}");
                    OnSpellSetsUpdated?.Invoke(spellSets);
                    return true;
                }
            }

            Log($"No empty slots in {currentSetIndex}");
            return false;
        }

        public void SetSpellInCurrentSet(int slotIndex, Spell spell)
        {
            if (slotIndex < 0 || slotIndex >= 4 || spellSets.Count == 0)
            {
                Debug.LogWarning($"Invalid slot index: {slotIndex}");
                return;
            }

            spellSets[currentSetIndex].Slots[slotIndex] = spell;
            OnSpellSetsUpdated?.Invoke(spellSets);
            Log($"Set {spell?.SpellName ?? "null"} in slot {slotIndex + 1}");
        }

        public Spell GetSpellInCurrentSet(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 4 || spellSets.Count == 0)
                return null;

            if (spellSets[currentSetIndex].Slots.Length > slotIndex)
                return spellSets[currentSetIndex].Slots[slotIndex];
            else return null;
        }

        public void NextSpellSet()
        {
            if (spellSets.Count <= 1) return;

            currentSetIndex = (currentSetIndex + 1) % spellSets.Count;
            OnCurrentSetChanged?.Invoke(CurrentSpellSet);
            Log($"Switched to spell set: {currentSetIndex}");
        }

        public void PreviousSpellSet()
        {
            if (spellSets.Count <= 1) return;

            currentSetIndex = (currentSetIndex - 1 + spellSets.Count) % spellSets.Count;
            OnCurrentSetChanged?.Invoke(CurrentSpellSet);
            Log($"Switched to spell set: {currentSetIndex}");
        }

        public void AddNewSpellSet()
        {
            int newSetNumber = spellSets.Count + 1;
            spellSets.Add(new SpellSet());
            OnSpellSetsUpdated?.Invoke(spellSets);
            Log($"Added new spell set: Set {newSetNumber}");
        }

        public void RemoveSpellSet(int index)
        {
            if (spellSets.Count <= 1 || index < 0 || index >= spellSets.Count)
                return;

            spellSets.RemoveAt(index);

            if (currentSetIndex >= spellSets.Count)
                currentSetIndex = Mathf.Max(0, spellSets.Count - 1);

            OnSpellSetsUpdated?.Invoke(spellSets);
            OnCurrentSetChanged?.Invoke(CurrentSpellSet);
            Log($"Removed spell set at index {index}");
        }

        private void Log(string message)
        {
            if (enableLogging)
                Debug.Log($"[PlayerSpellInventory] {message}");
        }
    }
}