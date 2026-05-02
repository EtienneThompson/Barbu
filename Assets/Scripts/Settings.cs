namespace Barbu
{
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
            /// The computers should be more difficult than in easy mode.
            /// </summary>
            Normal,

            /// <summary>
            /// The computers should be the most difficult.
            /// </summary>
            Hard,
        }

        public enum BackColorOption
        {
            /// <summary>
            /// Use a black back color for cards.
            /// </summary>
            Black,

            /// <summary>
            /// Use a red back color for cards.
            /// </summary>
            Red,

            /// <summary>
            /// Use a blue back color for cards.
            /// </summary>
            Blue,
        }

        public enum MenuSide
        {
            /// <summary>
            /// The menu appears on the right side of the screen.
            /// </summary>
            Right,

            /// <summary>
            /// The menu appears on the left side of the screen.
            /// </summary>
            Left,
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

        public static BackColorOption[] BackColors => (BackColorOption[])Enum.GetValues(typeof(BackColorOption));

        public static BackColorOption BackColorPreference
        {
            get
            {
                return (BackColorOption)PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.BackColorOption);
            }

            set
            {
                PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.BackColorOption, (int)value);
            }
        }

        public static MenuSide[] MenuSides => (MenuSide[])Enum.GetValues(typeof(MenuSide));

        public static MenuSide MenuSidePreference
        {
            get
            {
                return (MenuSide)PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.MenuSideOption);
            }

            set
            {
                PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.MenuSideOption, (int)value);
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
}
