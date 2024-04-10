using System.Collections.Generic;

public class PlayTrickWorkflow : BaseWorkflow<PlayTrickArguments>
{
    protected override Dictionary<string, IStep> Steps => new Dictionary<string, IStep>
    {
        ["player1cardstep"] = new PlayCardStep(),
        ["player2cardstep"] = new PlayCardStep(),
        ["player3cardstep"] = new PlayCardStep(),
        ["player4cardstep"] = new PlayCardStep(),
        [nameof(ResolveTrickStep)] = new ResolveTrickStep(),
    };

    public PlayTrickWorkflow()
        : base()
    {
        this.Arguments = new StepArguments<PlayTrickArguments>
        {
            Data = new PlayTrickArguments
            {
                gameStates = new GameState[4],
                currentGameState = 0,
            },
        };
    }
}