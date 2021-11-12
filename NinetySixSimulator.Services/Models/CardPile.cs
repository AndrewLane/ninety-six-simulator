namespace NinetySixSimulator.Services.Models;

public class CardPile
{
    private static readonly Random rng = new Random();

    public List<Card> Cards { get; set; } = new List<Card>();

    /// <summary>
    /// Shuffles a list based on https://stackoverflow.com/questions/273313/randomize-a-listt
    /// </summary>
    public void Shuffle()
    {
        int n = Cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = Cards[k];
            Cards[k] = Cards[n];
            Cards[n] = value;
        }
    }

    public Card Draw()
    {
        var card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }
}
