using UnityEngine;
using EchoesOfAetherion.Enemies.Core;
using EchoesOfAetherion.Player.Components;
using EchoesOfAetherion.StateMachine;
using EchoesOfAetherion.Enemies.StoneScorpion.States;
using EchoesOfAetherion.CameraUtils;
using UnityEditor.Callbacks;

namespace EchoesOfAetherion.Enemies.StoneScorpion
{
    [RequireComponent(typeof(Rigidbody2D), typeof(StoneScorpionAnimations))]
    public class StoneScorpionController : EnemyController<StoneScorpionController>
    {
        public StoneScorpionAnimations Animator { get; private set; }
        [field: SerializeField] public LayerMask PlayerMask { get; private set; }
        [field: SerializeField] public float DetectionRadius { get; private set; } = 120f;

        //! tmp while we don't have a class for movement.
        public Vector2 Velocity => rb.linearVelocity;
        public GameObject Target { get; set; }
        [field: SerializeField] public CameraFollow CameraFollow { get; private set; }

        private Rigidbody2D rb;

        protected override void Awake()
        {
            base.Awake();

            Animator = GetComponent<StoneScorpionAnimations>();
            CameraFollow = Camera.main.GetComponent<CameraFollow>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void SetupStateMachine()
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