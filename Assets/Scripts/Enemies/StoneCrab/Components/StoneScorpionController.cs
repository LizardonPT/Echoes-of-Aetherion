using UnityEngine;
using EchoesOfAetherion.Enemies.Core;
using EchoesOfAetherion.Enemies.Data;
using EchoesOfAetherion.Player.Components;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Enemies.StoneScorpion.States;
using EchoesOfAetherion.CameraUtils;

namespace EchoesOfAetherion.Enemies.StoneScorpion
{
    [RequireComponent(typeof(Rigidbody2D), typeof(StoneScorpionAnimations))]
    public class StoneScorpionController : EnemyController<StoneScorpionController>
    {
        public EnemyData Data { get; private set; }
        public StoneScorpionAnimations Animator { get; private set; }
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private float detectionRadius = 5f;

        private PlayerController player;
        private CameraFollow cameraFollow;

        private Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();
            Data = new EnemyData();
            Animator = GetComponent<StoneScorpionAnimations>();
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected override void Update()
        {
            base.Update();
            Data.Velocity = rb.linearVelocity;

            player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerMask)
                ?.GetComponent<PlayerController>();

            if (player != null && !Data.IsFollowingTarget)
            {
                Data.Target = player.transform;
                cameraFollow?.AddTarget(transform);
            }
            else if (player == null && Data.IsFollowingTarget)
            {
                cameraFollow?.RemoveTarget(transform);
                Data.Target = null;
            }
        }

        protected override void SetupStateMachine()
        {
            stateMachine = new FiniteStateMachine<StoneScorpionController>(this);
            stateMachine.AddState<StoneScorpionIdleState>(new StoneScorpionIdleState());
            stateMachine.ChangeState<StoneScorpionIdleState>();
        }
    }
}