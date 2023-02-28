using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameState
{
    void Start();
    void GoNext();
    int GetId();
}
