using System.Collections;
using System.Collections.Generic;

public interface IGameState
{
    string PlayerId { get; }

    void Start();
    void CleanUp();
    void GoNext();
    void SetNextState(GameState next);
}
