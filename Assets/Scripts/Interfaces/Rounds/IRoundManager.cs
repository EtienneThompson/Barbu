using System.Collections;
using System.Collections.Generic;

public interface IRoundManager
{
    void PreRound();
    void StartRound();
    void CleanupRound();
    void NextRound(Hand[] hands);
    void Destroy();
}
