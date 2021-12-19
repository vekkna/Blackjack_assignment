using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    internal class Hand
    {
        public List<Card> Cards { get; private set; }

        public int Value
        {
            get
            {
                int value = (from card in Cards select (int)card.Rank).Sum();
                int numAces = (from card in Cards where card.Rank is Rank.Ace select card).Count();
                while (numAces > 0 && value > 21)
                {
                    numAces -= 1;
                    value -= 10;
                }
                return value;
            }
        }

        public Hand()
        {
            Cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public override string ToString()
        {
            return $"{Cards[0]} and {Cards[1]}";
        }
    }
}