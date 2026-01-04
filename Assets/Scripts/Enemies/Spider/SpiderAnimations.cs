using EchoesOfEtherion.Enemies.Core;
using EchoesOfEtherion.Game;
using EchoesOfEtherion.Game.Core;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.Spider
{
    public class SpiderAnimations : TickRegistor
    {
        [SerializeField] private Animator anim;

        private Agent agent;

        private static readonly int xHash = Animator.StringToHash("x");
        private static readonly int yHash = Animator.StringToHash("y");
        private static readonly int biteHash = Animator.StringToHash("Bite");
        private static readonly int webShootHash = Animator.StringToHash("WebShoot");
        private static readonly int hurtHash = Animator.StringToHash("Hurt");

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
            anim.SetFloat(xHash, lookDirection.x);
            anim.SetFloat(yHash, lookDirection.y);
        }

        private void OnValidate()
        {
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }

        public void Bite()
        {
            anim.SetTrigger(biteHash);
        }

        public void WebShoot()
        {
            anim.SetTrigger(webShootHash);
        }

        public void Hurt()
        {
            anim.SetTrigger(hurtHash);
        }
    }
}