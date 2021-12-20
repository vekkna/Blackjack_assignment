namespace Blackjack_NCrowley
{
    /// <summary>
    /// Represents a card, using the suit and rank defined in Deck.cs Rank's value is used by Hand.cs when getting the value of a hand
    /// </summary>
    internal class Card
    {
        // Properties

        public string Suit { get; }
        public string Rank { get; }

        /// <summary>
        /// Create a card - called in Deck.cs ctor
        /// </summary>
        /// <param name="suit">The card's suit</param>
        /// <param name="rank">The card's rank</param>
        public Card(string suit, string rank)
        {
            Suit = suit;
            Rank = rank;
        }

        /// <summary>
        /// String representation of the card
        /// </summary>
        /// <returns>String representation of the cards, for example, "Ace of Spades"</returns>
        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}