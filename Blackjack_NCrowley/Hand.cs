using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blackjack_NCrowley
{
    /// <summary>
    /// Player's (and Dealer's) hand, made up of cards, and which calculates cards' total value
    /// </summary>
    internal class Hand
    {
        // Properties

        // The contents of the hand
        public List<Card> Cards { get; }

        public int Value
        {
            get
            {
                // Add the values of the cards in hand, using the Rank enum's values (faces are ten, ace is eleven)
                int value = (from card in Cards select Deck.Ranks[card.Rank]).Sum();
                // Count how many aces are in the hand
                int numAces = (from card in Cards where card.Rank is "Ace" select card).Count();
                // If hand is bust and it contains "big" aces
                while (numAces > 0 && value > 21)
                {
                    // change aces to "small" aces
                    numAces -= 1;
                    value -= 10;
                }
                return value;
            }
        }

        //ctor - no need for any others
        public Hand()
        {
            Cards = new List<Card>();
        }

        /// <summary>
        /// Add a card to the hand
        /// </summary>
        /// <param name="card">The Card to add</param>
        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        /// <summary>
        /// Discards the hand, clearing the card array and returning the cards discarded
        /// </summary>
        /// <returns>The discarded Cards</returns>
        public List<Card> Discard()
        {
            List<Card> discards = new List<Card>(Cards);
            Cards.Clear();
            return discards;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Cards.ForEach(c => sb.Append(" " + c.ToString())); // format this better
            return sb.ToString();
        }
    }
}