using System.Collections;
using System.Collections.Generic;

public interface IRoundManager
{
    void NextRound(Hand[] hands);
    void SetStartingPlayer(GameState player);
    GameState GetPlayerFromId(string id);
    void Destroy();
}
