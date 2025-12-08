using EchoesOfEtherion.HealthSystem;
using EchoesOfEtherion.ManaSystem;
using EchoesOfEtherion.Player.Components;
using UnityEngine;

namespace EchoesOfEtherion.Spells.Data
{
    [CreateAssetMenu(fileName = "HealSpell", menuName = "Scriptable Objects/Spells/Heal Spell")]
    public class PureLight : Spell
    {
        [SerializeField] private int healAmount = 30;

        public override bool ExecuteSpell(PlayerSpellCaster caster)
        {
            if (cdTimer > 0f || caster.TryGetComponent(out ManaModule manaModule) && manaModule.CurrentMana < ManaCost)
                return false;

            manaModule.ConsumeMana(ManaCost);
            cdTimer = Cooldown;
            if (caster.TryGetComponent(out HealthModule healthModule))
            {
                healthModule.Heal(healAmount);
            }
            return true;
        }
    }
}
