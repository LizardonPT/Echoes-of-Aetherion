using UnityEngine;
using EchoesOfEtherion.StateMachine;
using FMODUnity;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionStingAttackState : IState<StoneScorpionController>
    {
        private float stingDuration = 0.7f;
        private float timer;
        private bool hasAttacked = false;
        private Vector2 stingDirection;
        private float stingSpeed = 120f;

        public void Enter(StoneScorpionController controller)
        {
            timer = 0f;
            hasAttacked = false;

            // Stop steering behaviors, we'll control movement manually
            controller.SeekBehaviour.IsActive = false;
            controller.OrbitBehaviour.IsActive = false;
            controller.StopBehaviour.IsActive = false;
            controller.ObstacleAvoidanceBehaviour.IsActive = false;
            controller.SeparationBehaviour.IsActive = false;

            // Set sting direction
            stingDirection = controller.LookDirection;

            //todo: Trigger sting animation
        }

        public void Update(StoneScorpionController controller)
        {
            timer += Time.deltaTime;

            // Launch forward during the first part of the attack
            if (timer < stingDuration * 0.4f)
            {
                controller.RB.linearVelocity = stingDirection * stingSpeed;

                // Perform sting attack mid-dash
                if (!hasAttacked && timer >= stingDuration * 0.3f)
                {
                    RuntimeManager.PlayOneShot(controller.Sting, controller.transform.position);
                    controller.PerformStingAttack();
                    controller.ResetAttackCooldown();
                    hasAttacked = true;
                }
            }
            else
            {
                // Slow down after attack
                controller.RB.linearVelocity = Vector2.Lerp(controller.Velocity, Vector2.zero, Time.deltaTime * 5f);
            }

            // Return to appropriate state after attack is complete
            if (timer >= stingDuration)
            {
                ReturnToCombatState(controller);
            }
        }

        public void FixedUpdate(StoneScorpionController controller) { }

        public void Exit(StoneScorpionController controller) { }

        private void ReturnToCombatState(StoneScorpionController controller)
        {
            if (controller.Target != null)
            {
                float distance = Vector2.Distance(controller.transform.position, controller.TargetPos);

                if (distance < controller.OrbitBehaviour.OrbitDistance)
                {
                    controller.StateMachine.ChangeState<StoneScorpionRotateState>();
                }
                else
                {
                    controller.StateMachine.ChangeState<StoneScorpionChaseState>();
                }
            }
            else
            {
                controller.StateMachine.ChangeState<StoneScorpionSearchState>();
            }
        }
    }
}