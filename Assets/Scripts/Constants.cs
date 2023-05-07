using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public const int MaxRounds = 6;

    public const int CardsPerPile = 4;

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
        public const string MenuButtons = "MenuButtons";
        public const string GamesMenu = "GamesMenu";
        public const string SettingsMenu = "SettingsMenu";
    }
}
