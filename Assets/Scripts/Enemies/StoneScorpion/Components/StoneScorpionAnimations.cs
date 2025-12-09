using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Game;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion
{
    public class StoneScorpionAnimations : TickRegistor
    {
        [SerializeField] private Animator animator;

        //todo: use real animations.
        //! tmp variable.
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Agent agent;

        private void Awake()
        {
            agent = GetComponent<Agent>();
        }


        public override void Tick()
        {
            if (agent == null)
                UpdateAnimation(Vector2.zero, Vector2.right);

            Vector2 vel = agent.RB.linearVelocity;
            Vector2 lookDir = agent.LookDirection;
            UpdateAnimation(vel, lookDir.normalized);
        }

        public void UpdateAnimation(Vector2 velocity, Vector2 lookDirection)
        {
            //todo: use real animations.
            if (lookDirection.x > 1e-5f)
            {
                spriteRenderer.flipX = true;
            }
            else if (lookDirection.x < -1e-5f)
            {
                spriteRenderer.flipX = false;
            }
        }

        private void OnValidate()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }
    }
}
