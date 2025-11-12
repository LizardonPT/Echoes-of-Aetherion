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

        [SerializeField]
        private SpellPage slot1;

        private List<SpellPage> spellPages;

        private void Awake()
        {
            spellPages = new List<SpellPage>();
        }

        public void AddSpellPage(SpellPage page)
        {
            if (!spellPages.Contains(page))
            {
                spellPages.Add(page);
                if (slot1 == null) slot1 = page;
                Log($"Page {page.SpellName} was added to the inventory.");
                if (enableLogging)
                {
                    string spells = "";
                    for (int i = 0; i < spellPages.Count; i++)
                    {
                        spells += spellPages[i].SpellName;
                        spells += i < spellPages.Count - 1 ? ", " : ".";
                    }
                    Log($"Inventory is now: {spells}");
                }
            }
        }
        public void RemoveSpellPage(SpellPage page)
        {
            if (spellPages.Contains(page)) spellPages.Remove(page);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out CollectableSpell collectableSpell))
            {
                Log($"Trigger page {collectableSpell.SpellPage.SpellName}");
                AddSpellPage(collectableSpell.SpellPage);
                Destroy(collectableSpell.gameObject);
            }
        }

        public SpellPage GetSpellInSlot(int i)
        {
            if (i == 1)
                return slot1;

            else return null;
        }

        private void Log(string message)
        {
            if (enableLogging)
                Debug.Log($"[PlayerInventory] {message}");
        }
    }
}
