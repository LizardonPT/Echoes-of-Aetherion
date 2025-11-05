using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion
{
    public class StoneScorpionAnimations : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        //todo: use real animations.
        //! tmp variable.
        [SerializeField] private SpriteRenderer spriteRenderer;

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
