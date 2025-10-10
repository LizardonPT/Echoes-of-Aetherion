using System;
using EchoesOfAetherion.StateMachine;
using UnityEngine;

namespace EchoesOfAetherion.Enemies.Core
{
    public class EnemyController<T> : MonoBehaviour where T : EnemyController<T>
    {
        protected FiniteStateMachine<T> stateMachine;

        protected virtual void Awake()
        {
            SetupStateMachine();
        }

        protected virtual void Update()
        {
            stateMachine?.Update();
        }

        protected virtual void FixedUpdate()
        {
            stateMachine?.FixedUpdate();
        }

        protected virtual void SetupStateMachine()
        {
            stateMachine = new FiniteStateMachine<T>((T)this);
        }
    }

}