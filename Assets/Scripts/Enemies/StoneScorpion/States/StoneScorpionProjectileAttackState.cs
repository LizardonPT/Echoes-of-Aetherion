using UnityEngine;
using EchoesOfEtherion.StateMachine;
using FMODUnity;

namespace EchoesOfEtherion.Enemies.StoneScorpion.States
{
    public class StoneScorpionProjectileAttackState : IState<StoneScorpionController>
    {
        private float attackDelay = 0.5f;
        private float timer;
        private bool hasAttacked = false;

        public void Enter(StoneScorpionController controller)
        {
            timer = 0f;
            hasAttacked = false;

            // Stop all movement
            controller.SeekBehaviour.IsActive = false;
            controller.OrbitBehaviour.IsActive = false;
            controller.StopBehaviour.IsActive = true;
            controller.ObstacleAvoidanceBehaviour.IsActive = false;
            controller.SeparationBehaviour.IsActive = false;

            if (controller.Target != null)
            {
                Vector2 dirToTarget = (controller.TargetPos - (Vector2)controller.transform.position).normalized;
                controller.LookDirection = dirToTarget;
            }

            RuntimeManager.PlayOneShot(controller.GatherRock, controller.transform.position);

            //todo: start projectile charge animation.
        }

        public void Update(StoneScorpionController controller)
        {
            timer += Time.deltaTime;

            if (!hasAttacked && timer >= attackDelay)
            {
                GameObject projectile = GameObject.Instantiate(
                    controller.ProjectilePrefab,
                    controller.ProjectileSpawnPoint.position,
                    Quaternion.identity
                );

                if (projectile.TryGetComponent<StoneBolder>(out var bolder))
                {
                    Vector2 targetPos = controller.TargetPos;
                    if (controller.Target.TryGetComponent(out Rigidbody2D targetRB))
                        targetPos += targetRB.linearVelocity;
                    bolder.Initialize(controller.ProjectileSpawnPoint.position, targetPos, controller.ProjectileDamage);
                }
                
                RuntimeManager.PlayOneShot(controller.RockThrow, controller.transform.position);

                controller.ResetAttackCooldown();
                hasAttacked = true;
            }

            if (timer >= attackDelay + 0.2f)
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