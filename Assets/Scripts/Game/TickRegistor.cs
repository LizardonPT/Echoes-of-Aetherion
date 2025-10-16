using UnityEngine;

namespace EchoesOfAetherion.Game
{
    public class TickRegistor : MonoBehaviour, ITickable
    {
        protected virtual void Start()
        {
            TickRegistration.Register(this);
        }

        public virtual void Tick()
        {

        }

        public virtual void FixedTick()
        {

        }

        protected virtual void OnDestroy()
        {
            TickRegistration.UnRegister(this);
        }
    }
}
