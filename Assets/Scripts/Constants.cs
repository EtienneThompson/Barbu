namespace Barbu
{
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
            public const string Hearts = "Hearts";
            public const string Queens = "Queens";
            public const string KingOfHearts = "KingOfHearts";
            public const string Piles = "Piles";
            public const string Nothing = "Nothing";
            public const string Everything = "Everything";
            public const int MaxRounds = 1;
        }

        public static class ChaosRoundManager
        {
            public const string GameName = "Chaos";
            public const int MaxRounds = 1;
        }

        public static class GameObjects
        {
            public const string ApplicationController = "ApplicationController";
            public const string GameBoard = "GameBoard";
            public const string ScoreMenuCanvas = "ScoreMenu";
            public const string MenuButtons = "MenuButtons";
            public const string GamesMenu = "GamesMenu";
            public const string SettingsMenu = "SettingsMenu";
            public const string RoundOverlay = "RoundOverlay";
            public const string InGamePoints = "InGamePoints";
            public const string HowToPlayScreen = "HowToPlayScreen";
            public const string SingleRoundMenu = "SingleRoundMenu";
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
            public const string ComputerDifficulty = "ComputerDifficulty";
            public const string BackColorOption = "BackColorOption";
            public const string SeenHowToPlayInstructions = "SeenHowToPlayInstructions";

            public const string TraditionalGamesPlayed = "TraditionalGamesPlayed";
            public const string TraditionalGamesFinished = "TraditionalGamesFinished";
            public const string TraditionalGamesWon = "TraditionalGamesWon";
            public const string SingleGamesPlayed = "SingleGamesPlayed";
            public const string SingleGamesFinished = "SingleGamesFinished";
            public const string SingleGamesWon = "SingleGamesWon";
            public const string ChaosGamesPlayed = "ChaosGamesPlayed";
            public const string ChaosGamesFinished = "ChaosGamesFinished";
            public const string ChaosGamesWon = "ChaosGamesWon";
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

        public static class RoundNames
        {
            public const string AcesRound = "No Aces";
            public const string ClubsRound = "No Clubs";
            public const string DiamondRound = "No Diamonds";
            public const string EightsRound = "No Eights";
            public const string EvensRound = "No Evens";
            public const string EverythingRound = "Everything";
            public const string FaceCardsRound = "No Face Cards";
            public const string FivesRound = "No Fives";
            public const string FoursRound = "No Fours";
            public const string HeartsRound = "No Hearts";
            public const string JacksRound = "No Jacks";
            public const string KingOfHeartsRound = "No King of Hearts";
            public const string KingsRound = "No Kings";
            public const string NinesRound = "No Nines";
            public const string NothingRound = "Nothing";
            public const string OddsRound = "No Odds";
            public const string PilesRound = "No Piles";
            public const string QueensRound = "No Queens";
            public const string SevensRound = "No Sevens";
            public const string SixesRound = "No Sixes";
            public const string SpadesRound = "No Spades";
            public const string TensRound = "No Tens";
            public const string ThreesRound = "No Threes";
            public const string TwosRound = "No Twos";
        }
    }
}