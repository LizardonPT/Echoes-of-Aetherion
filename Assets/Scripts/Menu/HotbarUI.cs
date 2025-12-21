using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.Spells;

namespace EchoesOfEtherion.Menu
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private List<Image> hotbarSlots;
        [SerializeField] private PlayerSpellInventory playerInventory;

        private void Awake()
        {
            if (playerInventory == null)
            {
                playerInventory = GetComponentInParent<PlayerSpellInventory>();
            }
r
            if (hotbarSlots.Count < 4)
            {
                Debug.LogWarning($"HotbarUI: Need at least 4 slots, but only found {hotbarSlots.Count}. Adding placeholder.");
                while (hotbarSlots.Count < 4)
                {
                    var newSlot = new GameObject("HotbarSlot").AddComponent<Image>();
                    newSlot.transform.SetParent(transform);
                    hotbarSlots.Add(newSlot);
                }
            }

            // Initialize all slots as inactive
            foreach (Image slot in hotbarSlots)
            {
                slot.gameObject.SetActive(false);
            }
        }

        private void OnSpellSetChanged(SpellSet spellSet)
        {
            if (spellSet == null)
            {
                Debug.LogWarning("HotbarUI: Received null spell set");
                return;
            }

            for (int i = 0; i < Mathf.Min(4, hotbarSlots.Count); i++)
            {
                Image slotImage = hotbarSlots[i];
                
                if (i < spellSet.Slots.Length && spellSet.Slots[i] != null)
                {
                    slotImage.sprite = spellSet.Slots[i].SpellIcon;
                    slotImage.gameObject.SetActive(true);
                    slotImage.color = Color.white;
                }
                else
                {
                    slotImage.sprite = null;
                    slotImage.gameObject.SetActive(false);
                }
            }
        }

        private void OnEnable()
        {
            if (playerInventory != null)
            {
                playerInventory.OnCurrentSetChanged += OnSpellSetChanged;
                
                if (playerInventory.CurrentSpellSet != null)
                {
                    OnSpellSetChanged(playerInventory.CurrentSpellSet);
                }
            }
        }

        private void OnDisable()
        {
            if (playerInventory != null)
            {
                playerInventory.OnCurrentSetChanged -= OnSpellSetChanged;
            }
        }
        private void Start()
        {
            if (playerInventory != null)
            {
                playerInventory.OnSpellSetsUpdated += OnSpellSetsUpdated;
            }
        }

        private void OnSpellSetsUpdated(List<SpellSet> allSets)
        {
            if (playerInventory != null && playerInventory.CurrentSpellSet != null)
            {
                OnSpellSetChanged(playerInventory.CurrentSpellSet);
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            playerInventory = GetComponentInParent<PlayerSpellInventory>();
            
            if (hotbarSlots == null || hotbarSlots.Count == 0)
            {
                hotbarSlots = new List<Image>();
                var childImages = GetComponentsInChildren<Image>(true);
                foreach (var image in childImages)
                {
                    if (image.gameObject != gameObject) // Don't include self
                    {
                        hotbarSlots.Add(image);
                    }
                }
                
                hotbarSlots.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
            }
        }
#endif
    }
}
