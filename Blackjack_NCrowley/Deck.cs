using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    /// <summary>
    /// Deck of cards
    /// </summary>
    internal class Deck
    {
        public static string[] Suits = { "Clubs", "Hearts", "Spades", "Diamonds" };

        // The names of the ranks (for printing) mapped to their values for determining hand values.
        public static Dictionary<string, int> Ranks = new()
        {
            { "Two", 2 },
            { "Three", 3 },
            { "Four", 4 },
            { "Five", 5 },
            { "Six", 6 },
            { "Seven", 7 },
            { "Eight", 8 },
            { "Nine", 9 },
            { "Ten", 10 },
            { "Jack", 10 },
            { "Queen", 10 },
            { "King", 10 },
            { "Ace", 11 }
        };

        // The cards in the deck as a queue for easy drawing from the top and restocking to the bottom
        private readonly Queue<Card> _contents;

        public Deck()
        {
            // Make a card of each suit/rank combination
            var cards = from suit in Suits from rank in Ranks.Keys select new Card(suit, rank);
            // Shuffle the cards and turn them into the deck
            _contents = new Queue<Card>(Shuffle(cards));
        }

        /// <summary>
        /// Takes the discards from each round, shuffles them, and adds them to the bottom of the deck
        /// </summary>
        /// <param name="cards">The discards from the last round</param>
        public void Restock(IEnumerable<Card> cards)
        {
            foreach (var card in Shuffle(cards))
            {
                _contents.Enqueue(card);
            }
        }

        public Card TakeTopCard()
        {
            return _contents.Dequeue();
        }

        /// <summary>
        /// Shuffles the deck, or part of one (for discards)
        /// </summary>
        /// <param name="cards">Cards to shuffle</param>
        /// <returns>Shuffled cards</returns>
        private List<Card> Shuffle(IEnumerable<Card> cards)
        {
            return cards.OrderBy(_ => new Random().NextDouble()).ToList();
        }
    }
}