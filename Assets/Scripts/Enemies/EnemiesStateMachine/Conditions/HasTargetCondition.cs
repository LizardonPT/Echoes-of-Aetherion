using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Player.Components;
using NaughtyAttributes;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions
{
    public class HasTargetCondition : BaseCondition
    {
        [SerializeField]
        private bool checkIfHaveTarget = true;

        protected override void OnInitialize()
        {
            
        }

        protected override void Evaluate()
        {
        }

        public override bool IsMet()
        {
            bool hasTarget = agent.Target != null;
            return checkIfHaveTarget ? hasTarget : !hasTarget;
        }
    }
}
