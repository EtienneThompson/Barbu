namespace Barbu.Gameplay
{
    public interface ICardFactory
    {
        Card CreateCard(string suit, string rank, string playerId);
    }
}
