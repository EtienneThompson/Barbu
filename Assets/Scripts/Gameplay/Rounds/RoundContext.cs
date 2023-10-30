using System.Collections.Generic;

public class RoundContext
{
    private IRound current;

    public RoundContext()
    {
    }

    public void Next()
    {
        current.GoNext();
    }

    public string CurrentName()
    {
        return current.Name;
    }

    public void SetState(IRound current)
    {
        this.current = current;
    }

    public int CalculatePointsInPile(Card[] pile)
    {
        return this.current.CalculatePointsInPile(pile);
    }

    public int CalculateCurrentPoints(List<Card[]> piles)
    {
        return this.current.CalculatePointsInAllPiles(piles);
    }

    public bool IsRoundOver(int round, Dictionary<string, int[]> points, int pilesPlayed)
    {
        return this.current.IsRoundOver(round, points, pilesPlayed);
    }

    public bool IsRoundPositive()
    {
        return this.current.IsRoundPositive();
    }

    public bool IsPointEarningCard(string cardName)
    {
        return this.current.IsPointEarningCard(cardName);
    }

    public int GetCardPointValue(string cardName)
    {
        return this.current.GetCardPointValue(cardName);
    }
}
