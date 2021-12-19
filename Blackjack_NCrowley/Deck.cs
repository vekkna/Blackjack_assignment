using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    internal enum Rank
    {
        Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack = Ten, Queen = Ten, King = Ten, Ace = 11
    }

    internal enum Suit
    {
        Clubs, Hearts, Spades, Diamonds
    }

    internal class Deck
    {
        public Queue<Card> Contents { get; private set; }
        private Random random = new Random();

        public Deck()
        {
            var cards = from Suit suit in Enum.GetValues(typeof(Suit))
                        from Rank rank in Enum.GetValues(typeof(Rank))
                        select new Card(suit, rank);

            Contents = new Queue<Card>(Shuffle(cards));
        }

        public Card TakeTopCard()
        {
            return Contents.Dequeue();
        }

        public void Restock(List<Card> cards)
        {
            Shuffle(cards).ForEach(card => Contents.Enqueue(card));
        }

        private List<Card> Shuffle(IEnumerable<Card> cards)
        {
            return cards.OrderBy(x => random.Next()).ToList();
        }
    }
}