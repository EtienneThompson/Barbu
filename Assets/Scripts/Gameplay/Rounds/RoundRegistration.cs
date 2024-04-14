namespace Barbu.Gameplay.Rounds
{
    using System;
    using System.Runtime.Serialization;
    using Barbu.Gameplay.Rounds.Rounds;
    using UnityEngine;

    public class RoundRegistration
    {
        public static Type[] registeredRounds = new Type[]
        {
        typeof(HeartsRound),
        typeof(DiamondsRound),
        typeof(SpadesRound),
        typeof(ClubsRound),
        typeof(TwosRound),
        typeof(ThreesRound),
        typeof(FoursRound),
        typeof(FivesRound),
        typeof(SixesRound),
        typeof(SevensRound),
        typeof(EightsRound),
        typeof(NinesRound),
        typeof(TensRound),
        typeof(JacksRound),
        typeof(QueensRound),
        typeof(KingOfHeartsRound),
        typeof(AcesRound),
        typeof(KingOfHeartsRound),
        typeof(PilesRound),
        typeof(NothingRound),
        typeof(FaceCardsRound),
        typeof(EvensRound),
        typeof(OddsRound),
        };

        public static Round GetRandomRound()
        {
            var index = (int)Mathf.Floor(UnityEngine.Random.value * registeredRounds.Length);
            return (Round)FormatterServices.GetUninitializedObject(registeredRounds[index]);
        }
    }
}