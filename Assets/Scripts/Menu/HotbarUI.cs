using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.Spells;
using Unity.VisualScripting;

namespace EchoesOfEtherion.Menu
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private List<Image> hotbarSlots;
        [SerializeField] private List<Image> hotbarSlotHighlights;

        [SerializeField] private PlayerInventory playerInventory;
        
        private void Awake()
        {
            if (playerInventory == null)
            {
                playerInventory = GetComponentInParent<PlayerInventory>();
            }

            foreach (Image slot in hotbarSlots)
            {
                slot.gameObject.SetActive(false);
            }

            foreach (Image highlight in hotbarSlotHighlights)
            {
                highlight.gameObject.SetActive(false);
            }
        }
        
        private void OnSlotsUpdated(Dictionary<int, Spell> slots)
        {
            for (int i = 0; i < hotbarSlots.Count; i++)
            {
                if (slots.TryGetValue(i + 1, out Spell spell) && spell != null)
                {
                    hotbarSlots[i].sprite = spell.SpellIcon;
                    hotbarSlots[i].gameObject.SetActive(true);
                }
                else
                {
                    hotbarSlots[i].sprite = null;
                    hotbarSlots[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnSelectedSpellChanged(Spell selectedSpell, int selectedIndex)
        {
            for (int i = 0; i < hotbarSlotHighlights.Count; i++)
            {
                hotbarSlotHighlights[i].gameObject.SetActive(i == selectedIndex - 1 && selectedSpell != null);
            }
        }

        private void OnEnable()
        {
            if (playerInventory != null)
            {
                playerInventory.SlotsUpdated += OnSlotsUpdated;
                playerInventory.SelectedSpellChanged += OnSelectedSpellChanged;
            }
                
        }

        private void OnDisable()
        {
            if (playerInventory != null)
            {
                playerInventory.SlotsUpdated -= OnSlotsUpdated;
                playerInventory.SelectedSpellChanged -= OnSelectedSpellChanged;
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            playerInventory = GetComponentInParent<PlayerInventory>();
        }
#endif
    }
}