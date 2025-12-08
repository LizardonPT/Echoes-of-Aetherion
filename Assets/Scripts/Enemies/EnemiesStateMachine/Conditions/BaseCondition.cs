using EchoesOfEtherion.Enemies.Core;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions
{
    public abstract class BaseCondition : MonoBehaviour
    {
        [Header("Condition Settings")]
        [SerializeField] protected float checkInterval = 0f;

        protected Agent agent;
        private float lastCheckTime;

        public void Initialize(Agent agent)
        {
            this.agent = agent;
            OnInitialize();
        }

        protected abstract void OnInitialize();

        public void OnUpdate()
        {
            if (Time.time - lastCheckTime < checkInterval) return;

            lastCheckTime = Time.time;
            Evaluate();
        }

        protected abstract void Evaluate();

        public abstract bool IsMet();
    }
}
