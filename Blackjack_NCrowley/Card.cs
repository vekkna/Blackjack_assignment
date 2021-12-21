namespace Blackjack_NCrowley
{
    /// <summary>
    /// Represents a card, using the suit and rank defined in Deck.cs.
    /// Rank's value is used by Hand.cs when getting the value of a hand.
    /// </summary>
    internal class Card
    {
        public string Suit { get; }
        public string Rank { get; }

        public Card(string suit, string rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}