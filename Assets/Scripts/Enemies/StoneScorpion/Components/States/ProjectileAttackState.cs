using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class ProjectileAttackState : BaseState
    {
        [Header("Attack Settings")]
        [SerializeField] private float attackDelay = 0.5f;
        [SerializeField] private float recoveryTime = 0.2f;
        
        private float timer;
        private bool hasAttacked;
        private StoneScorpionController scorpion;

        public override void OnEnter()
        {
            scorpion = agent as StoneScorpionController;

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
            RuntimeManager.PlayOneShot(scorpion.GatherRockSoundEvent, agent.transform.position);
            
            // Optional: Start charge animation
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

        public override void OnFixedUpdate()
        {
            // Keep velocity at zero during attack
            agent.RB.linearVelocity = Vector2.zero;
        }

        private void PerformProjectileAttack()
        {
            if (scorpion == null) return;
            
            // Spawn projectile
            GameObject projectile = Instantiate(
                scorpion.ProjectilePrefab,
                scorpion.ProjectileSpawnPoint.position,
                Quaternion.identity
            );

            // Initialize projectile
            // Note: You'll need to adapt this based on your projectile component
            if (projectile.TryGetComponent(out StoneBolder bolder))
            {
                Vector2 targetPos = agent.TargetPos;
                if (agent.Target.TryGetComponent(out Rigidbody2D targetRB))
                    targetPos += targetRB.linearVelocity;
                bolder.Initialize(scorpion.ProjectileSpawnPoint.position, targetPos, scorpion.ProjectileDamage);
            }
            
            // Play throw sound
            RuntimeManager.PlayOneShot(scorpion.RockThrowSoundEvent, agent.transform.position);
            
            // Reset attack cooldown
            scorpion.ResetAttackCooldown();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !enabled) return;
            
            // Draw attack timing
            Gizmos.color = Color.red;
            float progress = Mathf.Clamp01(timer / (attackDelay + recoveryTime));
            Vector3 pos = transform.position + Vector3.up * 1.5f;
            Gizmos.DrawWireCube(pos, new Vector3(1f, 0.2f, 0f));
            Gizmos.DrawCube(pos - new Vector3(0.5f - progress * 0.5f, 0f, 0f),
                          new Vector3(progress, 0.15f, 0f));
            
            // Draw projectile trajectory
            if (agent.Target != null && scorpion != null)
            {
                Gizmos.color = Color.magenta;
                Vector3 spawnPos = scorpion.ProjectileSpawnPoint.position;
                Vector3 targetPos = agent.TargetPos;
                Gizmos.DrawLine(spawnPos, targetPos);
                Gizmos.DrawWireSphere(targetPos, 10f);
            }
        }
#endif
    }
}