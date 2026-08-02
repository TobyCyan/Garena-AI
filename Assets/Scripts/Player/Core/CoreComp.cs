namespace Player.Core
{
    public sealed class CoreComp<T> where T : CoreComponent
    {
        private readonly Core core;
        private T component;

        public T Component
        {
            get
            {
                if (component == null)
                {
                    component = core.GetCoreComponent<T>();
                }

                return component;
            }
        }

        public CoreComp(Core core)
        {
            this.core = core;
        }
    }
}
