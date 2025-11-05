using EchoesOfEtherion.Enemies.Core;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.SteeringBehaviours
{
    [RequireComponent(typeof(Agent))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class SteeringBehaviour : MonoBehaviour, ISteeringBehaviour
    {
        [SerializeField] private float weight = 1f;
        [SerializeField] private bool isActive = true;

        protected Agent agent;
        protected Rigidbody2D rb;

        public float Weight => weight;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                OnActivate();
            }
        }

        protected Agent Agent => agent;
        protected Rigidbody2D Rb => rb;
        protected float MaxAccel => agent.MaxAccel;
        protected float MaxSpeed => agent.MaxSpeed;
        protected Vector2 Velocity => rb.linearVelocity;

        protected virtual void Start()
        {
            agent = GetComponent<Agent>();
            rb = GetComponent<Rigidbody2D>();
        }

        public abstract Vector2 GetSteering(GameObject target);

        protected virtual void OnActivate() { }

        public static Vector2 Deg2Vec(float angle)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float Vec2Deg(Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }
    }
}
