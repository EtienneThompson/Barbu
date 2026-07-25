namespace Barbu
{
    using System;
    using UnityEngine;

    public class StatisticsService : IStatisticsService
    {
        public int GetGamesFinished()
        {
            return PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.SingleGamesFinished);
        }

        public int GetGamesWon()
        {
            return PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.SingleGamesWon);
        }

        public void IncrementGamesPlayed(GameTypes type)
        {
            switch (type)
            {
                case GameTypes.Traditional:
                    var tradGamesPlayed = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.TraditionalGamesPlayed);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.TraditionalGamesPlayed, tradGamesPlayed + 1);
                    break;
                case GameTypes.Single:
                    var singleGamesPlayed = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.SingleGamesPlayed);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.SingleGamesPlayed, singleGamesPlayed + 1);
                    break;
                case GameTypes.Chaos:
                    var chaosGamesPlayed = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.ChaosGamesPlayed);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.ChaosGamesPlayed, chaosGamesPlayed + 1);
                    break;
                default:
                    throw new Exception("Invalid game type provided");
            }
        }

        public void IncrementGamesFinished(GameTypes type)
        {
            switch (type)
            {
                case GameTypes.Traditional:
                    var tradGamesFinished = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.TraditionalGamesFinished);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.TraditionalGamesFinished, tradGamesFinished + 1);
                    break;
                case GameTypes.Single:
                    var singleGamesFinished = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.SingleGamesFinished);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.SingleGamesFinished, singleGamesFinished + 1);
                    break;
                case GameTypes.Chaos:
                    var chaosGamesFinished = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.ChaosGamesFinished);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.ChaosGamesFinished, chaosGamesFinished + 1);
                    break;
                default:
                    throw new Exception("Invalid game type provided");
            }
        }

        public void IncrementGamesWon(GameTypes type)
        {
            switch (type)
            {
                case GameTypes.Traditional:
                    var tradGamesWon = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.TraditionalGamesWon);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.TraditionalGamesWon, tradGamesWon + 1);
                    break;
                case GameTypes.Single:
                    var singleGamesWon = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.SingleGamesWon);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.SingleGamesWon, singleGamesWon + 1);
                    break;
                case GameTypes.Chaos:
                    var chaosGamesWon = PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.ChaosGamesWon);
                    PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.ChaosGamesWon, chaosGamesWon + 1);
                    break;
                default:
                    throw new Exception("Invalid game type provided");
            }
        }
    }
}
