using System.Threading;
using EchoesOfEtherion.Enemies.EnemiesStateMachine;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.States;
using EchoesOfEtherion.HealthSystem;
using EchoesOfEtherion.Player.Components;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class ChargeAttackState : BaseState
    {
        [Header("Sting Attack Settings")]
        [SerializeField] private float stingSpeed = 120f;
        [SerializeField] private float stingDamage = 25;
        [SerializeField] private float stingRange = 60;
        [SerializeField] private float minStingCooldown = 3;
        [SerializeField] private float maxStingCooldown = 5;
        [SerializeField] private float windUpDuration = .15f;
        [SerializeField] private float endDuration = .15f;
        [SerializeField] private EventReference stingHitSoundEvent;
        [SerializeField] private EventReference stingAttackSoundEvent;

        [SerializeField] private TimerCondition timer;
        [SerializeField] private RangeCondition stingRangeCondition;
        [SerializeField] private UnityEvent OnWindUp;
        [SerializeField] private UnityEvent OnAttack;

        private bool hasAttacked;
        private float windUpTimer;

        private float attackResetTimer;

        protected override void OnInitialize()
        {
            timer.SetDuration(minStingCooldown, maxStingCooldown);

            stingRangeCondition.SetRange(stingRange);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer.StopTimer();
            windUpTimer = windUpDuration;
            hasAttacked = false;
            finished = false;

            OnWindUp?.Invoke();
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
                    OnAttack?.Invoke();
                }
            }
            else
            {
                attackResetTimer -= Time.deltaTime;
                if (attackResetTimer < 0)
                {
                    finished = true;
                }
            }
        }

        private void PerformStingAttack()
        {
            hasAttacked = true;
            attackResetTimer = endDuration;

            Vector2 pos = (Vector2)agent.transform.position + agent.LookDirection * stingRange * .5f;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, stingRange * .5f, agent.PlayerMask);

            PlayerController player = null;

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out PlayerController playerC))
                {
                    player = playerC;
                    break;
                }
            }

            if (player == null) return;

            agent.RB.AddForce(agent.LookDirection * stingSpeed, ForceMode2D.Impulse);

            if (player.TryGetComponent<HealthModule>(out var playerHealth))
            {
                playerHealth.Damage(gameObject, stingDamage, 150);
                RuntimeManager.PlayOneShot(stingHitSoundEvent, playerHealth.transform.position);
            }
        }
    }
}
