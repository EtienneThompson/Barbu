using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

public class RoundRegistration
{
    public static Type[] registeredRounds = new Type[]
    {
        typeof(HeartsRound),
        typeof(QueensRound),
        typeof(KingOfHeartsRound),
        typeof(PilesRound),
        typeof(NothingRound),
    };

    public static Round GetRandomRound()
    {
        var index = (int)Mathf.Floor(UnityEngine.Random.value * registeredRounds.Length);
        return (Round)FormatterServices.GetUninitializedObject(registeredRounds[index]);
    }
}
