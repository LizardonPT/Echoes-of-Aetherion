using System.Collections;
using EchoesOfEtherion.HealthSystem;
using UnityEngine;
using UnityEngine.AI;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions
{
    public class GotHitCondition : BaseCondition
    {
        [Header("Timer Settings")]
        [SerializeField] float timerDuration = .02f;

        private bool gotHit;
        private float lastHit;
        private HealthModule healthModule;

        private bool hadModule = true;
        protected override void OnInitialize()
        {
            healthModule = agent.GetComponent<HealthModule>();
            if (!hadModule)
                healthModule.Damaged += OnDamaged;
        }

        private void OnEnable()
        {
            if (healthModule != null)
                healthModule.Damaged += OnDamaged;
            else
                hadModule = false;
        }

        private void OnDisable()
        {
            healthModule.Damaged -= OnDamaged;
        }

        protected override void Evaluate()
        {
            if (gotHit)
            {
                if (Time.time - lastHit > timerDuration)
                    gotHit = false;
            }
        }

        public override bool IsMet()
        {
            return gotHit;
        }

        private void OnDamaged(DamageInfo info)
        {
            lastHit = Time.time;
            gotHit = true;
        }
    }
}
