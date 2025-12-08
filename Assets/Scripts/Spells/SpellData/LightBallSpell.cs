using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.ManaSystem;
using UnityEngine;
using EchoesOfEtherion.Spells.Runtime;

namespace EchoesOfEtherion.Spells.Data
{
    [CreateAssetMenu(fileName = "LightBallSpell", menuName = "Scriptable Objects/Spells/Light Ball Spell")]
    public class LightBallSpell : Spell
    {
        [SerializeField] private LightBallSpellRuntime spellRuntimePrefab;
        [field: SerializeField] public float Damage { get; private set; } = 20f;
        [field: SerializeField] public float KnockbackAmount { get; private set; } = 80;
        [field: SerializeField] public float Speed { get; private set; } = 10f;
        [field: SerializeField] public int Range { get; private set; } = 50;

        public override bool ExecuteSpell(PlayerSpellCaster caster)
        {
            if (cdTimer > 0f || caster.TryGetComponent(out ManaModule manaModule) && manaModule.CurrentMana < ManaCost)
                return false;
            manaModule.ConsumeMana(ManaCost);
            cdTimer = Cooldown;
            Vector2 direction = caster.GetComponent<PlayerController>().LookDirection;
            LightBallSpellRuntime spellInstance = Instantiate(spellRuntimePrefab, caster.CasterPos.position, Quaternion.identity, caster.transform)
                                            .GetComponent<LightBallSpellRuntime>();

            spellInstance.ExecuteSpell(caster.CasterPos.position, direction);

            return true;
        }
    }
}