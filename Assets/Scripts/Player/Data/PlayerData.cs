using EchoesOfAetherion;
using UnityEngine;

namespace EchoesOfAetherion.Player.Data
{
    public class PlayerData
    {
        public Vector2 MovementInput { get; set; }
        public Vector2 LookDirection { get; set; }
        public bool IsMoving => MovementInput.magnitude > 1e-5f;
    }
}
