using EchoesOfAetherion;
using UnityEngine;

namespace EchoesOfAetherion.Enemies.Data
{
    public class EnemyData
    {
        public Vector2 Velocity { get; set; }
        public Transform Target { get; set; }
        public bool IsFollowingTarget => Target != null;
        public bool IsMoving => Velocity.magnitude > 1e-5f;
    }
}
