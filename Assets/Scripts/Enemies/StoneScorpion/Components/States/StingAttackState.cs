using System.Threading;
using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using EchoesOfEtherion.HealthSystem;
using EchoesOfEtherion.Player.Components;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StingAttackState : BaseState
    {
        [Header("Sting Attack Settings")]
        [SerializeField] private float stingSpeed = 120f;
        [SerializeField] private float stingDamage = 25;
        [SerializeField] private float stingRange = 60;
        [SerializeField] private float minStingCooldown = 3;
        [SerializeField] private float maxStingCooldown = 5;
        [SerializeField] private EventReference stingHitSoundEvent;
        [SerializeField] private EventReference stingAttackSoundEvent;

        [SerializeField] private TimerCondition timer;
        [SerializeField] private RangeCondition stingRangeCondition;

        private bool hasAttacked;
        private float windUpTimer;
        private Vector2 stingDirection;
        private StoneScorpionController scorpion;

        protected override void OnInitialize()
        {
            scorpion = agent as StoneScorpionController;
            timer.SetDuration(minStingCooldown, maxStingCooldown);
            timer.StartTimer();

            stingRangeCondition.SetRange(stingRange);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer.StopTimer();

            hasAttacked = false;
            finished = false;

            // Set sting direction
            stingDirection = agent.LookDirection;

            // Play sting sound
            RuntimeManager.PlayOneShot(stingAttackSoundEvent, agent.transform.position);
        }

        public override void OnExit()
        {
            base.OnExit();

            timer.SetDuration(minStingCooldown, maxStingCooldown);
            timer.StartTimer();
            hasAttacked = false;
        }

        public override void OnUpdate()
        {
            if (!hasAttacked)
            {
                windUpTimer -= Time.deltaTime;
                if (windUpTimer < 0)
                {
                    PerformStingAttack();
                }
            }
        }

        private void PerformStingAttack()
        {
            hasAttacked = true;
            finished = true;

            if (scorpion == null) return;


            PlayerController player = FindAnyObjectByType<PlayerController>();

            if (player == null) return;

            agent.RB.AddForce(agent.LookDirection * stingSpeed, ForceMode2D.Impulse);

            if (Vector2.Distance(agent.transform.position, player.transform.position) <= stingRange)
            {
                if (player.TryGetComponent<HealthModule>(out var playerHealth))
                {
                    playerHealth.Damage(gameObject, stingDamage, 150);
                    RuntimeManager.PlayOneShot(stingHitSoundEvent, playerHealth.transform.position);
                }
            }

        }
    }
}
