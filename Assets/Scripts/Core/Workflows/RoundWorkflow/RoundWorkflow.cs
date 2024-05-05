namespace Barbu
{
    using Barbu.Core.Workflows;
    using Barbu.Interfaces.Core.Workflows;
    using System.Collections.Generic;

    public class RoundWorkflow : BaseWorkflow<RoundArguments>
    {
        protected override Dictionary<string, IStep<RoundArguments>> Steps => new Dictionary<string, IStep<RoundArguments>>
        {
        };
    }
}
