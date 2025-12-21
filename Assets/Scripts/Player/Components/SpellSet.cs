using System;
using EchoesOfEtherion.Spells;
using UnityEngine;

namespace EchoesOfEtherion.Player.Components
{
    [Serializable]
    public class SpellSet
    {
        public Spell[] Slots => slots;

        [SerializeField] private Spell[] slots = new Spell[4];

        public void SetSpell(int i, Spell spell)
        {
            slots[i] = spell;
        }
    }
}

