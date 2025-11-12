using UnityEngine;

namespace EchoesOfEtherion.Spells
{
    public interface IProjectileSpell
    {
        void ExecuteSpell(Vector2 position, Vector2 direction);
    }
}