using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
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

        [SerializeField] private LayerMask playerDamageMask;
        public StoneScorpionAnimations Animator { get; private set; }

        public override string EnemyType => "StoneScorpion";
        public GameObject ProjectilePrefab => projectilePrefab;
        private GameObject fakeTarget;
        public GameObject LastSeenTarget { get; private set; }
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
