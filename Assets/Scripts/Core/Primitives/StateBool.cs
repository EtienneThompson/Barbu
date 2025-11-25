namespace Barbu
{
    public struct StateBool
    {
        private bool value;

        public StateBool(bool initValue)
        {
            this.value = initValue;
        }

        // Implicit conversion from bool to StateBool.
        public static implicit operator StateBool(bool v) => new StateBool(v);

        // Implicit conversion from StateBool to bool.
        public static implicit operator bool(StateBool v) => v.value;

        public void Toggle()
        {
            this.value = !value;
        }

        public void Enable()
        {
            this.value = true;
        }

        public void Disable()
        {
            this.value = false;
        }

        public void Set(bool value)
        {
            this.value = value;
        }
    }
}
