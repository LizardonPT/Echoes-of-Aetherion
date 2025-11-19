using EchoesOfEtherion.Player.Components;
using UnityEngine;

namespace EchoesOfEtherion.Spells
{
    [CreateAssetMenu(fileName = "LightBallSpell", menuName = "Scriptable Objects/Spells/Light Ball Spell")]
    public class LightBallSpell : Spell
    {
        [SerializeField] private LightBallSpellRuntime spellRuntimePrefab;
        public override bool ExecuteSpell(PlayerSpellCaster caster)
        {
            if (cdTimer > 0f)
                return false;
            cdTimer = Cooldown;
            Vector2 direction = caster.GetComponent<PlayerController>().LookDirection;
            LightBallSpellRuntime spellInstance = Instantiate(spellRuntimePrefab, caster.transform.position, Quaternion.identity, caster.transform)
                                            .GetComponent<LightBallSpellRuntime>();

            spellInstance.ExecuteSpell(caster.transform.position, direction);
            
            return true;
        }
    }
}