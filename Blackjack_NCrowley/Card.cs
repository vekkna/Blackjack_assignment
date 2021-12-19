using System;

namespace Blackjack_NCrowley
{
    internal class Card // TODO should this be a record?
    {
        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
            int rankIndex = Array.IndexOf(Enum.GetValues(typeof(Rank)), rank);
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}