using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public const int CardsInDeck = 52;

    public const int CardsInHand = 13;

    public const int NumPilesPerRound = 13;

    public const int CardsPerPile = 4;

    public static class CardSuits
    {
        public const string Heart = "Heart";
        public const string Diamond = "Diamond";
        public const string Spade = "Spade";
        public const string Club = "Club";
    }

    public static class TraditionalRoundManager
    {
        public const string GameName = "Traditional";
        public const int MaxRounds = 6;
    }

    public static class SingleRoundManager
    {
        public const string GameName = "Single";
        public const int MaxRounds = 1;
    }

    public static class ChaosRoundManager
    {
        public const string GameName = "Chaos";
        public const int MaxRounds = 1;
    }

    public static class GameObjects
    {
        public const string GameBoard = "GameBoard";
        public const string ScoreMenuCanvas = "ScoreMenuCanvas";
        public const string MenuButtons = "MenuButtons";
        public const string GamesMenu = "GamesMenu";
        public const string SettingsMenu = "SettingsMenu";
        public const string RoundOverlay = "RoundOverlay";
        public const string InGamePoints = "InGamePoints";
    }

    public static class PlayerIds
    {
        public const string Player1 = "1";
        public const string Player2 = "2";
        public const string Player3 = "3";
        public const string Player4 = "4";
    }

    public static class PlayerPrefsKeys
    {
        public const string SortingOptions = "SortingOptions";
    }

    public static class AdGameIds
    {
        public const string AppleGameId = "5285615";
        public const string AppleBannerId = "Banner_iOS";
        public const string AppleInterstitialId = "Interstitial_iOS";
        public const string AppleRewardedId = "Rewarded_iOS";
        public const string AndroidGameId = "5285614";
        public const string AndroidBannerId = "Banner_Android";
        public const string AndroidInterstitialId = "Interstitial_Android";
        public const string AndroidRewardedId = "Rewarded_Android";
    }
}
