using UnityEngine;

namespace EchoesOfEtherion.HealthSystem
{
    public class DamageInfo
    {
        //? Maybe another type?
        public GameObject Damager { get; private set; }
        public float DamageAmount { get; private set; }
        public Vector2 DamageSourcePos { get; private set; }
        public float KnockbackAmount { get; private set; }
        public float StunTime { get; private set; }

        public DamageInfo(GameObject damager, float damageAmount, Vector2 damageSourcePos, float knockbackAmount, float stunTime = 0)
        {
            Damager = damager;
            DamageAmount = damageAmount;
            DamageSourcePos = damageSourcePos;
            KnockbackAmount = knockbackAmount;
            StunTime = stunTime;
        }
    }
}