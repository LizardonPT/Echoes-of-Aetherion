using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Player.Components;
using NaughtyAttributes;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions
{
    public class SmartRangeCondition : BaseCondition
    {
        public enum RangeType
        {
            InRange,
            OurOfRange,
        }

        [Header("Range Settings")]
        [SerializeField] private RangeType rangeType = RangeType.InRange;
        [SerializeField] private float range = 120f;

#if UNITY_EDITOR
        [ShowIf(nameof(IsInRange)), SerializeField]
#endif
        private bool tryFindTarget;

        [Header("LOS")]
        [SerializeField, Range(0, 360)] private float visionAngle = 90f;
        [SerializeField] private bool requireLineOfSight = true;

#if UNITY_EDITOR
        private bool IsInRange => rangeType == RangeType.InRange;
#endif

        private bool conditionMet = false;
        private PlayerController foundPlayer;

        private PlayerController playerInstance;

        protected override void OnInitialize()
        {
            playerInstance = FindAnyObjectByType<PlayerController>();
        }

        protected override void Evaluate()
        {
            conditionMet = false;

            EvaluateDistance();
        }

        private void EvaluateDistance()
        {
            if (tryFindTarget)
            {
                if (agent.Target != null)
                {
                    conditionMet = true;
                    return;
                }

                if (TrySearchForTarget(out PlayerController player))
                {
                    foundPlayer = player;
                    conditionMet = true;
                }
            }
            else
            {
                if (agent.Target == null)
                    return;

                Vector2 here = agent.transform.position;
                Vector2 targetPos = agent.TargetPos;
                float distance = Vector2.Distance(here, targetPos);

                if (distance <= range && rangeType == RangeType.InRange)
                {
                    conditionMet = true;
                    return;
                }
                else if (distance > range && rangeType == RangeType.OurOfRange)
                {
                    conditionMet = true;
                    return;
                }
            }
        }

        private bool TrySearchForTarget(out PlayerController target)
        {
            target = null;

            if (foundPlayer)
                return false;

            Vector2 origin = agent.transform.position;
            Vector2 dirToTarget = ((Vector2)playerInstance.transform.position - origin).normalized;

            if (requireLineOfSight)
            {
                if (Vector2.Angle(agent.LookDirection, dirToTarget) > visionAngle)
                    return false;

                LayerMask rayMask = (agent.PlayerMask | agent.EnvironmentMask) & ~agent.EnemyMask;
                RaycastHit2D rayHit = Physics2D.Raycast(origin, dirToTarget.normalized, range, rayMask);

                if (rayHit.collider != null &&
                    rayHit.collider.TryGetComponent(out PlayerController playerController))
                {
                    target = playerController;
                    return true;
                }
            }
            else
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, agent.PlayerMask);

                foreach (Collider2D col in hits)
                {
                    if (col.TryGetComponent(out PlayerController player))
                    {
                        target = player;
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool IsMet()
        {
            if (tryFindTarget)
            {
                if (foundPlayer != null)
                    agent.Target = foundPlayer;
                else
                    return false;
            }

            return conditionMet;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            bool inRange = rangeType == RangeType.InRange;
            if (!inRange && tryFindTarget)
                tryFindTarget = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Agent agent = GetComponentInParent<Agent>();
            if (agent != null)
                Gizmos.DrawWireSphere(agent.transform.position, range);
        }
#endif
    }
}
