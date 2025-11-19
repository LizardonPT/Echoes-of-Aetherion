using EchoesOfEtherion.Player.Components;
using UnityEngine;

namespace EchoesOfEtherion.Spells
{
    public abstract class Spell : ScriptableObject
    {
        [field: SerializeField]
        public int ManaCost { get; protected set; }
        [field: SerializeField]
        public float Cooldown { get; protected set; }
        [field: SerializeField]
        public string SpellName { get; protected set; }

        [field: SerializeField] public string AnimationName { get; protected set; } = "Attack";

        protected float cdTimer = 0f;

        public abstract bool ExecuteSpell(PlayerSpellCaster caster);
        public virtual void UpdateSpell(PlayerSpellCaster caster)
        {
            cdTimer -= Time.deltaTime;
            if (cdTimer < 0f)
            {
                cdTimer = 0f;
            }
        }
    }
}
