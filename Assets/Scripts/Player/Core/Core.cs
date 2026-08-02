using System.Collections.Generic;
using UnityEngine;

namespace Player.Core
{
    public class Core : MonoBehaviour
    {
        [field: SerializeField] public GameObject Root { get; private set; }

        private readonly List<CoreComponent> components = new();

        private void Awake()
        {
            if (Root == null)
            {
                Root = transform.parent != null ? transform.parent.gameObject : gameObject;
            }

            foreach (CoreComponent component in GetComponentsInChildren<CoreComponent>(true))
            {
                RegisterComponent(component);
            }
        }

        public void RegisterComponent(CoreComponent component)
        {
            if (component != null && !components.Contains(component))
            {
                components.Add(component);
            }
        }

        public void LogicUpdate()
        {
            for (int i = components.Count - 1; i >= 0; i--)
            {
                if (components[i] == null)
                {
                    components.RemoveAt(i);
                    continue;
                }

                if (components[i].isActiveAndEnabled)
                {
                    components[i].LogicUpdate();
                }
            }
        }

        public T GetCoreComponent<T>() where T : CoreComponent
        {
            foreach (CoreComponent component in components)
            {
                if (component is T match)
                {
                    return match;
                }
            }

            T childComponent = GetComponentInChildren<T>(true);
            RegisterComponent(childComponent);
            return childComponent;
        }

        public T GetCoreComponent<T>(ref T value) where T : CoreComponent
        {
            if (value == null)
            {
                value = GetCoreComponent<T>();
            }

            return value;
        }
    }
}
