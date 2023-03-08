using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameState
{
    int PlayerId { get; }

    void Start();
    void GoNext();
    int GetId();
}
