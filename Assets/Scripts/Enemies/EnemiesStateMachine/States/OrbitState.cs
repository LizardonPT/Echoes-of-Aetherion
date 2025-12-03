using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.States
{
    public class OrbitState : BaseState
    {
        [SerializeField] private float orbitDistance = 90f;
        [SerializeField] private float orbitSpeed = 0.8f;
        private int orbitDirection;

        public override void OnEnter()
        {
            orbitDirection = Random.Range(0, 2) == 0 ? 1 : -1;
        }

        public override void OnFixedUpdate()
        {
            if (agent.Target == null) return;

            Vector2 pos = agent.transform.position;
            Vector2 toTarget = agent.TargetPos - pos;

            Vector2 tangent = new Vector2(-toTarget.y, toTarget.x).normalized * orbitDirection;

            Vector2 orbitPoint = (Vector2)agent.TargetPos + tangent * orbitDistance;
            agent.MoveToPosition(orbitPoint, orbitSpeed);

            agent.LookDirection = toTarget.normalized;
        }
    }
}