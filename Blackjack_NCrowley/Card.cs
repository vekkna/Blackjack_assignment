using System;

namespace Blackjack_NCrowley
{
    internal class Card // TODO should this be a record?
    {
        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }
        public int Value { get; private set; } // TODO maybe cards don't know their values, the game does?

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
            int rankIndex = Array.IndexOf(Enum.GetValues(typeof(Rank)), rank);
            Value = rankIndex < 10 ? rankIndex + 1 : 10;
        }

        public override string ToString()
        {
            return $"{Rank} of {Suit}";
        }
    }
}