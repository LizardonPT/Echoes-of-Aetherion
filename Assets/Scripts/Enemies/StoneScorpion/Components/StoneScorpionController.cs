using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.HealthSystem;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion
{
    [RequireComponent(typeof(StoneScorpionAnimations))]
    public class StoneScorpionController : Agent
    {
        [field: SerializeField] public float CoolDownTime { get; private set; } = 2;
        [Header("Attacks")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float projectileDamage = 20f;


        [SerializeField] private float stingAttackRange = 64f;
        [SerializeField] private float stingAttackRadius = 16f;
        [SerializeField] private float stingDamage = 25f;
        [SerializeField] private float attackCooldown = 3f;
        [SerializeField] private LayerMask playerDamageMask;
        [field: SerializeField] public EventReference RockThrowSoundEvent { get; private set; }
        [field: SerializeField] public EventReference GatherRockSoundEvent { get; private set; }
        [field: SerializeField] public EventReference StingSoundEvent { get; private set; }
        [field: SerializeField] public EventReference StingHitSoundEvent { get; private set; }
        public StoneScorpionAnimations Animator { get; private set; }

        public override string EnemyType => "StoneScorpion";
        public GameObject ProjectilePrefab => projectilePrefab;
        public float ProjectileDamage => projectileDamage;

        private GameObject fakeTarget;
        public GameObject LastSeenTarget { get; private set; }
        private float lastAttackTime = 0;
        public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;
        public float StingAttackRange => stingAttackRange;
        public float StingAttackRadius => stingAttackRadius;
        public float StingDamage => stingDamage;
        public LayerMask PlayerDamageMask => playerDamageMask;
        public Transform ProjectileSpawnPoint => projectileSpawnPoint;

        protected override void Awake()
        {
            base.Awake();

            Animator = GetComponent<StoneScorpionAnimations>();
        }
        
        public override void Tick()
        {
            base.Tick();
            Animator.UpdateAnimation(RB.linearVelocity, LookDirection);
        }

        public override void FixedTick()
        {
            base.FixedTick();
        }

        public void PerformStingAttack()
        {
            // Circle cast to detect players in sting range
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(
                transform.position + (Vector3)LookDirection * stingAttackRange,
                stingAttackRadius,
                playerDamageMask
            );

            foreach (Collider2D playerCollider in hitPlayers)
            {
                HealthModule playerHealth = playerCollider.GetComponent<HealthModule>();
                if (playerHealth != null)
                {
                    playerHealth.Damage(gameObject, stingDamage, 150);
                    RuntimeManager.PlayOneShot(StingHitSoundEvent, playerHealth.transform.position);
                }
            }
        }

        public void ResetAttackCooldown()
        {
            lastAttackTime = Time.time;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (fakeTarget != null)
            {
                Destroy(fakeTarget);
            }
        }
    }
}
