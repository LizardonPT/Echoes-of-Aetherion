using EchoesOfEtherion.HealthSystem;
using EchoesOfEtherion.Player.Components;
using UnityEngine;

namespace EchoesOfEtherion.Spells
{
    [CreateAssetMenu(fileName = "HealSpell", menuName = "Scriptable Objects/Spells/Heal Spell")]
    public class HealSpell : Spell
    {
        [SerializeField] private int healAmount = 30;

        public override bool ExecuteSpell(PlayerSpellCaster caster)
        {
            if (cdTimer > 0f)
                return false;

            cdTimer = Cooldown;
            if (caster.TryGetComponent(out HealthModule healthModule))
            {
                healthModule.Heal(healAmount);
            }
            return true;
        }
    }
}
