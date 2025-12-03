// using UnityEngine;
// using EchoesOfEtherion.StateMachine;
// using EchoesOfEtherion.Player.Components;
// using FMODUnity;
// using EchoesOfEtherion.Enemies.Core;

// namespace EchoesOfEtherion.Enemies.StoneScorpion.States
// {
//     public class StoneScorpionDamagedState : IState<StoneScorpionController>
//     {
//         private float timer = 0;

//         public void Enter(StoneScorpionController controller)
//         {
//             timer = controller.StunTime;

//             RaycastHit2D[] hits = Physics2D.CircleCastAll(
//                 controller.transform.position,
//                 controller.DetectionRadius,
//                 controller.LookDirection,
//                 controller.EnemyMask);

//             foreach (RaycastHit2D hit in hits)
//             {
//                 if (hit.collider == null) continue;
//                 if (hit.collider.TryGetComponent(out Agent agent))
//                 {
//                     agent.SignalEnemyHit();
//                 }
//             }
//         }

//         public void Update(StoneScorpionController controller)
//         {
//             timer -= Time.deltaTime;

//             if (timer <= 0)
//             {
//                 if (controller.Target != null)
//                 {
//                     controller.StateMachine.ChangeState<StoneScorpionChaseState>();
//                 }
//                 else controller.StateMachine.ChangeState<StoneScorpionIdleState>();
//             }
//         }

//         public void FixedUpdate(StoneScorpionController controller) { }

//         public void Exit(StoneScorpionController controller) { }

//     }
// }
