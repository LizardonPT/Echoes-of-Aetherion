using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class ProjectileAttackState : BaseState
    {
        [Header("Attack Settings")]
        [SerializeField] private StoneBolder stoneBolderPrefab;
        [SerializeField] private float damage = 30;
        [SerializeField] private float attackDelay = 0.5f;
        [SerializeField] private float minCooldown;
        [SerializeField] private float maxCooldown;
        [SerializeField] private EventReference gatherRockSoundEvent;
        [SerializeField] private EventReference rockThrowSoundEvent;
        [SerializeField] private TimerCondition cooldown;

        private float timer;
        private bool hasAttacked;
        private StoneScorpionController scorpion;

        protected override void OnInitialize()
        {
            scorpion = agent as StoneScorpionController;

            cooldown.SetDuration(minCooldown, maxCooldown);
            cooldown.StartTimer();
        }

        public override void OnEnter()
        {
            finished = false;

            timer = 0f;
            hasAttacked = false;

            // Stop movement
            agent.RB.linearVelocity = Vector2.zero;

            // Face the target
            if (agent.Target != null)
            {
                Vector2 dirToTarget = (agent.TargetPos - (Vector2)agent.transform.position).normalized;
                agent.LookDirection = dirToTarget;
            }

            // Play charge sound
            RuntimeManager.PlayOneShot(gatherRockSoundEvent, agent.transform.position);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;

            // Perform attack after delay
            if (!hasAttacked && timer >= attackDelay)
            {
                PerformProjectileAttack();
                hasAttacked = true;
            }

            // Update animation
            if (scorpion != null)
            {
                scorpion.Animator.UpdateAnimation(agent.RB.linearVelocity, agent.LookDirection);
            }
        }


        private void PerformProjectileAttack()
        {
            if (scorpion == null) return;

            GameObject projectile = Instantiate(
                stoneBolderPrefab.gameObject,
                scorpion.ProjectileSpawnPoint.position,
                Quaternion.identity
            );

            if (projectile.TryGetComponent(out StoneBolder bolder))
            {
                Vector2 targetPos = agent.TargetPos;
                if (agent.Target.TryGetComponent(out Rigidbody2D targetRB))
                    targetPos += targetRB.linearVelocity;
                bolder.Initialize(scorpion.ProjectileSpawnPoint.position, targetPos, damage);
            }

            RuntimeManager.PlayOneShot(rockThrowSoundEvent, agent.transform.position);

            finished = true;
        }
    }
}