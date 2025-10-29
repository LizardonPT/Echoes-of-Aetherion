using UnityEngine;

namespace EchoesOfEtherion.Enemies.SteeringBehaviours
{
    public interface ISteeringBehaviour
    {
        float Weight { get; }
        bool IsActive { get; set; }
        
        Vector2 GetSteering(GameObject target);
    }
}
