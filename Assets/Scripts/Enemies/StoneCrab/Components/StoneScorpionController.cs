using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.SteeringBehaviours;
using EchoesOfEtherion.Enemies.StoneScorpion.States;
using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion
{
    [RequireComponent(typeof(StoneScorpionAnimations))]
    [RequireComponent(typeof(SeekBehaviour))]
    [RequireComponent(typeof(StopBehaviour))]
    [RequireComponent(typeof(ObstacleAvoidanceBehaviour))]
    [RequireComponent(typeof(OrbitBehaviour))]
    [RequireComponent(typeof(SeparationBehaviour))]
    public class StoneScorpionController : Agent
    {
        public StoneScorpionAnimations Animator { get; private set; }
        public FiniteStateMachine<StoneScorpionController> StateMachine { get; private set; }

        public SeekBehaviour SeekBehaviour { get; private set; }
        public StopBehaviour StopBehaviour { get; private set; }
        public OrbitBehaviour OrbitBehaviour { get; private set; }
        public ObstacleAvoidanceBehaviour ObstacleAvoidanceBehaviour { get; private set; }
        public SeparationBehaviour SeparationBehaviour { get; private set; }

        private GameObject fakeTarget;
        public GameObject LastSeenTarget { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Animator = GetComponent<StoneScorpionAnimations>();
            SeekBehaviour = GetComponent<SeekBehaviour>();
            StopBehaviour = GetComponent<StopBehaviour>();
            OrbitBehaviour = GetComponent<OrbitBehaviour>();
            ObstacleAvoidanceBehaviour = GetComponent<ObstacleAvoidanceBehaviour>();
            SeparationBehaviour = GetComponent<SeparationBehaviour>();

            CreateFakeTarget();
            SetupStateMachine();
        }

        public override void Tick()
        {
            base.Tick();
            StateMachine.Update();
        }

        public override void FixedTick()
        {
            base.FixedTick();
            StateMachine.FixedUpdate();
        }

        private void CreateFakeTarget()
        {
            fakeTarget = new GameObject("LastSeenPosition")
            {
                hideFlags = HideFlags.HideInHierarchy
            };
            LastSeenTarget = fakeTarget;
        }

        public void SetLastSeenPosition(Vector2 position)
        {
            if (fakeTarget != null)
            {
                fakeTarget.transform.position = position;
            }
        }

        public void ClearLastSeenPosition()
        {
            if (fakeTarget != null)
            {
                fakeTarget.transform.position = new Vector3(9999, 9999, 9999);
            }
        }

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<StoneScorpionController>(this);
            StateMachine.AddState<StoneScorpionIdleState>(new StoneScorpionIdleState());
            StateMachine.AddState<StoneScorpionChaseState>(new StoneScorpionChaseState());
            StateMachine.AddState<StoneScorpionRotateState>(new StoneScorpionRotateState());
            StateMachine.AddState<StoneScorpionSearchState>(new StoneScorpionSearchState());
            StateMachine.ChangeState<StoneScorpionIdleState>();
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
