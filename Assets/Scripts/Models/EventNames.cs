namespace Barbu.Models
{
    public enum EventNames
    {
        /// <summary>
        /// Event fired when the game should pause.
        /// </summary>
        PauseGame,

        /// <summary>
        /// Event fired when the game should resume.
        /// </summary>
        ResumeGame,

        /// <summary>
        /// Event fired when a card is played.
        /// </summary>
        PlayCard,

        /// <summary>
        /// Event fired when the round is over.
        /// </summary>
        ScoreMenuDismissed,

        /// <summary>
        /// Event fired when an individual card in a pile has completely resolved.
        /// </summary>
        CardInPileResolved,

        /// <summary>
        /// Event fired when 4 cards have been played into a pile.
        /// </summary>
        PileResolved,

        /// <summary>
        /// Event fired when the round animation is finished.
        /// </summary>
        RoundAnimationFinished,

        /// <summary>
        /// Event fired when the winning animation is finished.
        /// </summary>
        WinnerAnimationFinished,
    }
}
