using UnityEngine;

namespace Player.Core
{
    public abstract class CoreComponent : MonoBehaviour
    {
        protected Core Core { get; private set; }

        protected virtual void Awake()
        {
            Core = GetComponentInParent<Core>();
            Core?.RegisterComponent(this);
        }

        public virtual void LogicUpdate()
        {
        }
    }
}
