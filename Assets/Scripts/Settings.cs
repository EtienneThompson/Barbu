using System;
using UnityEngine;

public class Settings
{
    public enum SortingOptions
    {
        /// <summary>
        /// Cards are presented in whatever order they are dealt.
        /// </summary>
        None = 0,

        /// <summary>
        /// Cards are ordered from smallest to largest, regardless of suit.
        /// </summary>
        LowToHigh = 1,

        /// <summary>
        /// Cards are ordered from largest to smallest, regardless of suit.
        /// </summary>
        HighToLow = 2,

        /// <summary>
        /// Cards are ordered from smallest to largest within their suit.
        /// </summary>
        SuitLowToHigh = 3,

        /// <summary>
        /// Cards are ordered from largest to smallest within their suit.
        /// </summary>
        SuitHighToLow = 4,

        /// <summary>
        /// Cards are ordered from smallest to largest within their suit, with the suit colors alternating.
        /// </summary>
        SuitLowToHighAlternating = 5,

        /// <summary>
        /// Cards are ordered from largest to smallest within their suit, with the suit colors alternating.
        /// </summary>
        SuitHighToLowAlternating = 6,
    }

    public enum ComputerDifficulty
    {
        /// <summary>
        /// The computers should be relatively easy to beat.
        /// </summary>
        Easy,

        /// <summary>
        /// The computers should be more difficult than in normal mode.
        /// </summary>
        Normal,
    }

    public static SortingOptions[] HandSortingOptions => (SortingOptions[])Enum.GetValues(typeof(SortingOptions));

    public static SortingOptions SortingPreference
    {
        get
        {
            return (SortingOptions)PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.SortingOptions);
        }

        set
        {
            PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.SortingOptions, (int)value);
        }
    }

    public static ComputerDifficulty[] ComputerDifficulties => (ComputerDifficulty[])Enum.GetValues(typeof(ComputerDifficulty));

    public static ComputerDifficulty ComputerDifficultyPreference
    {
        get
        {
            return (ComputerDifficulty)PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.ComputerDifficulty);
        }

        set
        {
            PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.ComputerDifficulty, (int)value);
        }
    }

    public static bool HasSeenHowToPlayByDefault()
    {
        return PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.SeenHowToPlayInstructions) == 1;
    }

    public static void SetSeenHowToPlayByDefault()
    {
        PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.SeenHowToPlayInstructions, 1);
    }
}
