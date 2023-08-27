using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerStateFactory
{
    public static GameState GetComputerStateFromSettings()
    {
        return new EasyComputerState();
    }
}
