using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StingAttackState : BaseState
    {
        [Header("Sting Attack Settings")]
        [SerializeField] private float stingDuration = 0.7f;
        [SerializeField] private float stingSpeed = 120f;
        [SerializeField] private float attackPoint = 0.3f;

        private float timer;
        private bool hasAttacked;
        private Vector2 stingDirection;
        private StoneScorpionController scorpion;


        public override void OnEnter()
        {
            scorpion = agent as StoneScorpionController;
            
            timer = 0f;
            hasAttacked = false;

            // Set sting direction
            stingDirection = agent.LookDirection;

            // Play sting sound
            RuntimeManager.PlayOneShot(scorpion.StingSoundEvent, agent.transform.position);

            // Optional: Start sting animation
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / stingDuration;

            // Dash forward during first 40% of attack
            if (normalizedTime < 0.4f)
            {
                agent.RB.linearVelocity = stingDirection * stingSpeed;

                // Perform attack at the attack point
                if (!hasAttacked && normalizedTime >= attackPoint)
                {
                    PerformStingAttack();
                    hasAttacked = true;
                }
            }
            else
            {
                // Slow down after dash
                agent.RB.linearVelocity = Vector2.Lerp(agent.RB.linearVelocity, Vector2.zero, Time.deltaTime * 5f);
            }

            // Update animation
            if (scorpion != null)
            {
                scorpion.Animator.UpdateAnimation(agent.RB.linearVelocity, agent.LookDirection);
            }
        }

        public override void OnFixedUpdate()
        {
            // Movement is handled in Update
        }

        private void PerformStingAttack()
        {
            if (scorpion == null) return;

            // Perform sting attack using controller method
            scorpion.PerformStingAttack();

            // Reset attack cooldown
            scorpion.ResetAttackCooldown();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !enabled) return;
            
            // Draw sting attack range and direction
            Gizmos.color = Color.red;
            Vector3 startPos = agent.transform.position;
            Vector3 endPos = startPos + (Vector3)stingDirection * stingSpeed * 0.4f;
            Gizmos.DrawLine(startPos, endPos);
            
            // Draw sting attack radius at the end
            if (scorpion != null)
            {
                Gizmos.DrawWireSphere(endPos, scorpion.StingAttackRadius);
            }
            
            // Draw attack timing
            Gizmos.color = Color.yellow;
            float progress = Mathf.Clamp01(timer / stingDuration);
            Vector3 pos = agent.transform.position + Vector3.up * 2f;
            Gizmos.DrawWireCube(pos, new Vector3(1f, 0.2f, 0f));
            Gizmos.DrawCube(pos - new Vector3(0.5f - progress * 0.5f, 0f, 0f),
                          new Vector3(progress, 0.15f, 0f));
        }
#endif
    }
}