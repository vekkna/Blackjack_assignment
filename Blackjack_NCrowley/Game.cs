using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_NCrowley
{
    internal class Game
    {
        private Player[] players;
        private int activePlayerIndex;
        private int startingPlayerIndex;
        private Deck deck;

        public void StartRound()
        {
            deck = new Deck();
            activePlayerIndex += 1;
        }

        public Card Draw()
        {
            try
            {
                return deck.Contents.Pop();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Card> Draw(int n)
        {
            try
            {
                var cards = new List<Card>();
                for (; n > 0; n--)
                {
                    cards.Add(deck.Contents.Pop());
                }
                return cards;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}