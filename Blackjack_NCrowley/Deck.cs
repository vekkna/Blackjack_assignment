using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    internal enum Rank
    {
        Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
    }

    internal enum Suit
    {
        Clubs, Hearts, Spades, Diamonds
    }

    internal class Deck
    {
        public Stack<Card> Contents;

        public Deck()
        {
            var sorted = from Suit suit in Enum.GetValues(typeof(Suit)) from Rank rank in Enum.GetValues(typeof(Rank)) select new Card(suit, rank);
            var random = new Random();
            var shuffled = sorted.OrderBy(x => random.Next()).ToList();
            Contents = new Stack<Card>(shuffled);
        }
    }
}