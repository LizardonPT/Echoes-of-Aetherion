using System.Collections.Generic;
using UnityEngine;

public class Grimoire : MonoBehaviour
{
    [Header("Todas as Páginas Conhecidas")]
    public List<SpellPage> allPages = new List<SpellPage>();

    [Header("Slots Ativos (Teclas 1-4, por exemplo)")]
    public List<SpellPage> activeSpells = new List<SpellPage>();
    public int maxActiveSlots = 4;

    [SerializeField] private GameObject grimoirePanel;

    public void Show()
    {
        grimoirePanel.SetActive(true);
    }

    public void Hide()
    {
        grimoirePanel.SetActive(false);
    }
    
    // --- Gestão de Páginas ---
    public void AddPage(SpellPage newPage)
    {
        if (!allPages.Contains(newPage))
            allPages.Add(newPage);
    }

    public void EquipPage(SpellPage page, int slotIndex)
    {
        if (!allPages.Contains(page)) return;
        if (slotIndex < 0 || slotIndex >= maxActiveSlots) return;

        // Expande lista se necessário
        while (activeSpells.Count < maxActiveSlots)
            activeSpells.Add(null);

        activeSpells[slotIndex] = page;
    }

    public void UnequipPage(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= activeSpells.Count) return;
        activeSpells[slotIndex] = null;
    }

    public void CastSpell(int slotIndex, GameObject caster)
    {
        if (slotIndex < 0 || slotIndex >= activeSpells.Count) return;
    }
}
