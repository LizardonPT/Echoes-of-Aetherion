using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion.Conditions
{
    public class CanAttackCondition : BaseCondition
    {
        [Header("Attack Settings")]
        [SerializeField] private AttackType attackType = AttackType.Sting;
        [SerializeField] private float range = 64f;

        private StoneScorpionController scorpion;
        private bool conditionMet;

        public enum AttackType
        {
            Sting,
            Projectile
        }

        protected override void OnInitialize()
        {
            scorpion = agent as StoneScorpionController;
        }

        protected override void Evaluate()
        {
            conditionMet = false;

            if (scorpion == null || agent.Target == null) return;

            // Check if we can attack (cooldown)
            if (!scorpion.CanAttack) return;

            // Check distance based on attack type
            float distance = Vector2.Distance(agent.transform.position, agent.TargetPos);

            switch (attackType)
            {
                case AttackType.Sting:
                    conditionMet = distance <= scorpion.StingAttackRange;
                    break;
                case AttackType.Projectile:
                    conditionMet = distance <= range; // Use custom range or controller's range
                    break;
            }
        }

        public override bool IsMet()
        {
            return conditionMet;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !enabled) return;

            // Draw attack range
            Gizmos.color = attackType == AttackType.Sting ? Color.red : Color.blue;
            Gizmos.DrawWireSphere(agent.transform.position,
                attackType == AttackType.Sting && scorpion != null ?
                scorpion.StingAttackRange : range);
        }
#endif
    }
}