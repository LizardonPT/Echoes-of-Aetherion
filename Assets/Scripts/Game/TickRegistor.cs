using UnityEngine;

namespace EchoesOfEtherion.Game
{
    public class TickRegistor : MonoBehaviour, ITickable
    {

        protected virtual void Start()
        {
            TickController.Instance.Register(this);
        }

        public virtual void Tick()
        {

        }

        public virtual void FixedTick()
        {

        }

        protected virtual void OnDestroy()
        {
            TickController.Instance.UnRegister(this);
        }
    }
}
