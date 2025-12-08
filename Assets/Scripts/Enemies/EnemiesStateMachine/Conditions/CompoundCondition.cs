using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions
{
    public abstract class CompoundCondition : BaseCondition
    {
        [Header("Compound Conditions")]
        [SerializeField] protected List<BaseCondition> conditions = new();

        protected override void OnInitialize()
        {
            
        }
        protected override void Evaluate()
        {

        }

        public override bool IsMet()
        {
            return EvaluateConditions();
        }

        protected abstract bool EvaluateConditions();
    }
}