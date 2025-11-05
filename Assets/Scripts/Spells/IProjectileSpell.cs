using UnityEngine;

namespace EchoesOfEtherion.Spells
{
    public interface IProjectileSpell
    {
        //todo: Make caster a class or an interface.
        void ExecuteSpell(GameObject caster, Vector2 direction);
    }
}