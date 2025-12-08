using EchoesOfEtherion.Core;
using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.Core
{
    public class ProjectileAttackState : BaseState
    {
        [Header("Attack Settings")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float chargeTime = 0.5f;
        [SerializeField] private float minCooldown = 1f;
        [SerializeField] private float maxCooldown = 2f;

#if UNITY_EDITOR
        [ShowIf(nameof(HasChargeTime))]
#endif
        [SerializeField] private EventReference chargeSoundEvent;


#if UNITY_EDITOR
        public bool HasChargeTime => chargeTime > 0;
#endif
        [SerializeField] private EventReference throwSoundEvent;

        [SerializeField] private TimerCondition cooldown;

        private float timer;
        private bool hasAttacked;

        protected override void OnInitialize()
        {
            cooldown.SetDuration(minCooldown, maxCooldown);
        }

        public override void OnEnter()
        {
            finished = false;
            timer = 0f;
            hasAttacked = false;

            agent.RB.linearVelocity = Vector2.zero;

            if (agent.Target != null)
            {
                Vector2 dirToTarget = (agent.TargetPos - (Vector2)agent.transform.position).normalized;
                agent.LookDirection = dirToTarget;
            }
            if (chargeTime > 0)
                RuntimeManager.PlayOneShot(chargeSoundEvent, agent.transform.position);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (!hasAttacked && timer >= chargeTime)
            {
                PerformProjectileAttack();
                hasAttacked = true;
            }
        }

        private void PerformProjectileAttack()
        {
            if (projectilePrefab == null || agent.Target == null) return;

            Projectile proj = Instantiate(projectilePrefab, agent.transform.position, Quaternion.identity);
            proj.Initialize(agent.transform.position, agent.TargetPos);

            RuntimeManager.PlayOneShot(throwSoundEvent, agent.transform.position);
            finished = true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (chargeTime < 0)
                chargeTime = 0;
        }
#endif
    }
}
