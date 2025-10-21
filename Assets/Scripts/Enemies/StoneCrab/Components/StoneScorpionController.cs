using UnityEngine;
using EchoesOfEtherion.StateMachine;
using EchoesOfEtherion.Enemies.StoneScorpion.States;
using EchoesOfEtherion.Extentions;
using EchoesOfEtherion.Game;

namespace EchoesOfEtherion.Enemies.StoneScorpion
{
    [RequireComponent(typeof(StoneScorpionAnimations), typeof(Movement))]
    public class StoneScorpionController : TickRegistor
    {
        public StoneScorpionAnimations Animator { get; private set; }
        public Movement Movement { get; private set; }


        [field: SerializeField] public LayerMask PlayerMask { get; private set; }
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }
        [field: SerializeField] public LayerMask EnvironmentMask { get; private set; }

        [field: SerializeField] public float DetectionRadius { get; private set; } = 120f;
        [field: SerializeField] public float AttackDistance { get; private set; } = 60f;
        [field: SerializeField, Range(0, 360)] public int LookAngle { get; private set; } = 45;

        public Vector2 LookDirection = Vector2.right;
        public GameObject Target { get; set; }
        public Vector2 TargetPos => new(Target.transform.position.x, Target.transform.position.y + 6);

        public FiniteStateMachine<StoneScorpionController> StateMachine { get; private set; }

        private void Awake()
        {
            Animator = GetComponent<StoneScorpionAnimations>();
            Movement = GetComponent<Movement>();
            SetupStateMachine();
        }

        public override void Tick() => StateMachine?.Update();
        public override void FixedTick() => StateMachine?.FixedUpdate();

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<StoneScorpionController>(this);
            StateMachine.AddState<StoneScorpionIdleState>(new StoneScorpionIdleState());
            StateMachine.AddState<StoneScorpionChaseState>(new StoneScorpionChaseState());
            StateMachine.AddState<StoneScorpionRotateState>(new StoneScorpionRotateState());
            StateMachine.ChangeState<StoneScorpionIdleState>();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector2 pos = transform.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pos, DetectionRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pos, AttackDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + LookDirection.normalized * DetectionRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + LookDirection.Rotate(LookAngle).normalized * DetectionRadius);
            Gizmos.DrawLine(pos, pos + LookDirection.Rotate(-LookAngle).normalized * DetectionRadius);
        }
#endif
    }
}
