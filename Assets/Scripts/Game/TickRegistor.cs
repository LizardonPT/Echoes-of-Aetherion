using UnityEngine;

namespace EchoesOfAetherion.Game
{
    public class TickRegistor : MonoBehaviour, ITickable
    {
        [SerializeField]
        private TickChannel tickChannel;

        protected virtual void Start()
        {
            tickChannel.Register(this);
        }

        public virtual void Tick()
        {

        }

        public virtual void FixedTick()
        {

        }

        protected virtual void OnDestroy()
        {
            tickChannel.UnRegister(this);
        }
    }
}
