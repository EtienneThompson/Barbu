using System.Collections;
using System.Collections.Generic;
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
    }

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
}
