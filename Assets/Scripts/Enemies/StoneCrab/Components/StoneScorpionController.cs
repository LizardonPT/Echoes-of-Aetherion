using UnityEngine;
using EchoesOfAetherion.Player.Components;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Enemies.StoneScorpion.States;
using EchoesOfAetherion.CameraUtils;
using EchoesOfAetherion.Game;

namespace EchoesOfAetherion.Enemies.StoneScorpion
{
    [RequireComponent(typeof(Rigidbody2D), typeof(StoneScorpionAnimations))]
    public class StoneScorpionController : TickRegistor
    {
        public StoneScorpionAnimations Animator { get; private set; }
        [field: SerializeField] public LayerMask PlayerMask { get; private set; }
        [field: SerializeField] public float DetectionRadius { get; private set; } = 120f;

        //! tmp while we don't have a class for movement.
        public Vector2 Velocity => rb.linearVelocity;
        public GameObject Target { get; set; }

        public FiniteStateMachine<StoneScorpionController> StateMachine { get; private set; }

        private Rigidbody2D rb;

        private void Awake()
        {
            SetupStateMachine();

            Animator = GetComponent<StoneScorpionAnimations>();
            rb = GetComponent<Rigidbody2D>();
        }


        public override void Tick() => StateMachine?.Update();
        public override void FixedTick() => StateMachine?.FixedUpdate();

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<StoneScorpionController>(this);
            StateMachine.AddState<StoneScorpionIdleState>(new StoneScorpionIdleState());
            StateMachine.AddState<StoneScorpionChaseState>(new StoneScorpionChaseState());
            StateMachine.ChangeState<StoneScorpionIdleState>();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, DetectionRadius);
        }
#endif
    }
}