using EchoesOfEtherion.HealthSystem;
using FMODUnity;
using UnityEngine;
using EchoesOfEtherion.Core;

namespace EchoesOfEtherion.Enemies.StoneScorpion
{
    public class StoneBolder : Projectile
    {
        [Header("StoneBolder Settings")]
        [SerializeField] private float damage = 20f;
        [SerializeField] private float knockback = 150f;

        protected override void OnHitTarget(Collider2D hit)
        {
            if (hit.TryGetComponent(out HealthModule health))
            {
                health.Damage(gameObject, damage, knockback);
            }

            base.OnHitTarget(hit);
        }
    }
}
