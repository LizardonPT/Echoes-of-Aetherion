using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions
{
    public class OrCondition : CompoundCondition
    {
        protected override void OnInitialize()
        {
            
        }
        
        protected override bool EvaluateConditions()
        {
            if (conditions.Count == 0) return false;

            foreach (var condition in conditions)
            {
                if (condition == null) continue;
                if (condition.IsMet()) return true;
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || !enabled) return;

            // Visual feedback for OR condition
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.5f);
        }
#endif
    }
}