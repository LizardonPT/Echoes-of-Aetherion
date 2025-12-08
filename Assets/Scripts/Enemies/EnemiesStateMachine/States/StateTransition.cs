using System;
using System.Collections.Generic;
using EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.States
{
    [Serializable]
    public class StateTransition
    {
        [SerializeField] private BaseCondition condition;
        [SerializeField] private BaseState targetState;

        public BaseCondition Condition => condition;
        public BaseState TargetState => targetState;
    }
}
