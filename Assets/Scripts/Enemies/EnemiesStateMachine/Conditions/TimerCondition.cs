using UnityEngine;

namespace EchoesOfEtherion.Enemies.EnemiesStateMachine.Conditions
{
    public class TimerCondition : BaseCondition
    {
        [Header("Timer Settings")]
        [SerializeField] float duration = 3f;
        [SerializeField] private bool autoReset = true;

        private float startTime;
        private bool timerActive;
        bool conditionMet;

        protected override void OnInitialize()
        {
            
        }

        public void SetDuration(float d)
        {
            duration = d;
        }
        public void StartTimer()
        {
            startTime = Time.time;
            timerActive = true;
            conditionMet = false;
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
            if (!timerActive) return;

            conditionMet = Time.time - startTime >= duration;

            if (conditionMet && autoReset)
            {
                StartTimer();
            }
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
                float progress = Mathf.Clamp01((Time.time - startTime) / duration);
                Gizmos.color = Color.Lerp(Color.yellow, Color.green, progress);

                // Draw a progress bar above the enemy
                Vector3 pos = transform.position + Vector3.up * 1.5f;
                Gizmos.DrawWireCube(pos, new Vector3(1f, 0.2f, 0f));
                Gizmos.DrawCube(pos - new Vector3(0.5f - progress * 0.5f, 0f, 0f),
                              new Vector3(progress, 0.15f, 0f));
            }
        }
#endif
    }
}
