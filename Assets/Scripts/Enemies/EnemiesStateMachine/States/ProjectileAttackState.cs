using EchoesOfEtherion.Core;
using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace EchoesOfEtherion.Enemies.Core
{
    public class ProjectileAttackState : BaseState
    {
        [Header("Attack Settings")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private bool velocityPerdiction = true;
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

#if UNITY_EDIT
        [ShowIf(nameof(HasChargeTime))]
#endif
        [SerializeField] private UnityEvent OnCharge;
        [SerializeField] private UnityEvent OnAttack;

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
            {
                RuntimeManager.PlayOneShot(chargeSoundEvent, agent.transform.position);
                OnCharge?.Invoke();
            }
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            if (!hasAttacked && timer >= chargeTime)
            {
                PerformProjectileAttack();
                OnAttack?.Invoke();
                hasAttacked = true;
            }
        }

        private void PerformProjectileAttack()
        {
            if (projectilePrefab == null || agent.Target == null) return;

            Projectile proj = Instantiate(projectilePrefab, agent.transform.position, Quaternion.identity);
            Vector2 destination = agent.TargetPos;

            if (velocityPerdiction)
            {
                if (agent.Target.TryGetComponent(out Rigidbody2D targetBody))
                    destination += targetBody.linearVelocity;
            }

            proj.Initialize(agent.transform.position, destination);

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
