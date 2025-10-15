using System;
using EchoesOfAetherion.StateMachine;
using UnityEngine;

namespace EchoesOfAetherion.Enemies.Core
{
    public class EnemyController<T> : MonoBehaviour where T : EnemyController<T>
    {
        public FiniteStateMachine<T> StateMachine { get; protected set; }

        protected virtual void Awake()
        {
            SetupStateMachine();
        }

        protected virtual void Update()
        {
            StateMachine?.Update();
        }

        protected virtual void FixedUpdate()
        {
            StateMachine?.FixedUpdate();
        }

        protected virtual void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<T>((T)this);
        }
    }

}