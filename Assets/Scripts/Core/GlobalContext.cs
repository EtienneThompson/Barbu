namespace Barbu
{
    using Barbu.Core.Workflows.RoundWorkflow;

    public class GlobalContext
    {
        public static GlobalContext instance;

        public RoundWorkflow RoundWorkflow { get; set; }

        private GlobalContext()
        {
        }

        public static GlobalContext GetInstance()
        {
            if (instance == null)
            {
                instance = new GlobalContext();
            }

            return instance;
        }
    }
}
