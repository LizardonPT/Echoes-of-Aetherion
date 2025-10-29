using UnityEngine;
namespace EchoesOfEtherion.Enemies.SteeringBehaviours
{
    public class OrbitBehaviour : SteeringBehaviour
    {
        [field: SerializeField] public float OrbitDistance { get; private set; } = 60f;
        [SerializeField] private float offset = 25;
        [SerializeField] private float orbitStrength = 1f;
        [SerializeField] private float tolerance = 2f;
        [SerializeField] private float minChangeDirectionTime = 2;
        [SerializeField] private float maxChangeDirectionTime = 5;
        private int orbitDirection = 1;
        public int OrbitDirection { get => orbitDirection; set => orbitDirection = Mathf.Clamp(value, -1, 1); }
        private float startTime;
        private float randomChangeTime;

        protected override void Start()
        {
            base.Start(); OrbitDistance += Random.Range(-offset, offset);
        }
        protected override void OnActivate()
        {
            randomChangeTime = Random.Range(minChangeDirectionTime, maxChangeDirectionTime); startTime = Time.time;
        }

        public override Vector2 GetSteering(GameObject target)
        {
            Vector2 linear = Vector2.zero;
            if (startTime - Time.time >= randomChangeTime)
            {
                randomChangeTime = Random.Range(minChangeDirectionTime, maxChangeDirectionTime);
                OrbitDirection *= -1;
                startTime = Time.time;
            }
            if (target != null)
            {
                Vector2 targetPos = new(
                target.transform.position.x, target.transform.position.y + 6);
                Vector2 toTarget = targetPos - (Vector2)transform.position;
                float distance = toTarget.magnitude;
                Vector2 dirToTarget = toTarget.normalized;
                Vector2 tangent = new Vector2(-dirToTarget.y * orbitDirection, dirToTarget.x * orbitDirection) * orbitStrength;

                Vector2 radialCorrection = Vector2.zero;
                if (distance > OrbitDistance + tolerance)
                    radialCorrection = dirToTarget * 0.3f;
                else if (distance < OrbitDistance - tolerance)
                    radialCorrection = -dirToTarget * 0.3f;
                linear = (tangent + radialCorrection).normalized * MaxAccel;
            }
            return linear;
        }
    }
}