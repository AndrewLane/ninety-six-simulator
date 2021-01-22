namespace NinetySixSimulator.Services.Models
{
    public class PlayerGameState
    {
        public CardPile PlayPile { get; set; } = new CardPile();
        public CardPile GatherPile { get; set; } = new CardPile();
        public CardPile PlayedCards { get; set; } = new CardPile();
        public bool CannotContinueBecauseCantPlayEnoughCardsForWar { get; set; } = false;
    }
}
