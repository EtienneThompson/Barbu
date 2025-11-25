namespace Barbu
{
    public struct StateInteger
    {
        private int value;

        public StateInteger(int initValue)
        {
            this.value = initValue;
        }

        // Implicit conversion from int to StateInteger.
        public static implicit operator StateInteger(int v) => new StateInteger(v);

        // Implicit conversion from StateInteger to int.
        public static implicit operator int(StateInteger v) => v.value;

        public void Increment()
        {
            this.value++;
        }

        public void Increment(int v)
        {
            this.value += v;
        }

        public void Decrement()
        {
            this.value--;
        }

        public void Decrement(int v)
        {
            this.value -= v;
        }

        public void Set(int v)
        {
            this.value = v;
        }
    }
}
