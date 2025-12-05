using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions
{
    public class TimerCondition : BaseCondition
    {
        [Header("Timer Settings")]
        [SerializeField] float maxDuration = 3f;
        [SerializeField] float minDuration = 3;
        [SerializeField] private bool autoReset = true;
        [ShowIf(nameof(autoReset)), SerializeField] private float resetTime = 0.01f;
        [SerializeField] private bool autoUpdateCondition = true;
        [SerializeField] private bool autoStartWhenTarget;
        [SerializeField] private UnityEvent OnReachTime;
        public event Action ReachTime;

        private bool timerActive;
        private bool conditionMet;
        private float timer;

        public bool TimerEnded => conditionMet;
        public bool TimerActive => timerActive;

        private bool hadTarget;

        private void OnEnable()
        {
            checkInterval = 0;
        }

        protected override void OnInitialize()
        {
            hadTarget = false;
        }

        public void SetDuration(float min, float max)
        {
            minDuration = min;
            maxDuration = max;
        }

        public void StartTimer()
        {
            timerActive = true;
            conditionMet = false;

            timer = Random.Range(minDuration, maxDuration);
        }

        public void StopTimer()
        {
            timerActive = false;
        }

        public void ResetTimer()
        {
            StartTimer();
        }

        protected override void Evaluate()
        {
            if (autoStartWhenTarget)
            {
                if (agent.Target != null && !hadTarget)
                {
                    hadTarget = true;
                    StartTimer();
                }
                else if (agent.Target == null && hadTarget)
                {
                    hadTarget = false;
                    StopTimer();
                }
            }

            if (!timerActive) return;

            timer -= Time.deltaTime;

            bool reachedZero = timer < 0;

            if (reachedZero)
            {
                ReachTime?.Invoke();
                OnReachTime?.Invoke();

                if (autoReset)
                    Invoke(nameof(StartTimer), resetTime);

                if (autoUpdateCondition)
                    conditionMet = autoUpdateCondition;
            }
        }

        public void SetMetCondition()
        {
            conditionMet = true;
        }

        public override bool IsMet()
        {
            return conditionMet;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            if (timerActive)
            {
                float progress = Mathf.Clamp01(timer);
                Gizmos.color = Color.Lerp(Color.yellow, Color.green, progress);

                Vector3 pos = transform.position + Vector3.up * 1.5f;
                Gizmos.DrawWireCube(pos, new Vector3(1f, 0.2f, 0f));
                Gizmos.DrawCube(pos - new Vector3(0.5f - progress * 0.5f, 0f, 0f),
                              new Vector3(progress, 0.15f, 0f));
            }
        }
#endif
    }
}
