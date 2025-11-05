using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace EchoesOfEtherion.Spells
{
    public class CollectableSpell : MonoBehaviour
    {
        [field: SerializeField]
        public SpellPage SpellPage { get; private set; }
    }
}