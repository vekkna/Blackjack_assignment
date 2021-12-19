using System;
using System.Collections.Generic;

namespace Blackjack_NCrowley
{
    internal class Player
    {
        public string Name { get; private set; } // Todo use number word
        public int Cash { get; set; }
        public int Bet { get; set; }
        public Hand Hand { get; set; }


        public bool IsBust
        {
            get
            {
                return Hand.Value > 21;
            }
        }

        public Player(string name, int cash = 100)
        {  
            Name = name;
            Cash = cash;
            Hand = new Hand();
        }

        public Card Draw(Deck deck)
        {
            Card card = deck.TakeTopCard();
            Hand.AddCard(card);
            return card;
        }

        public void Draw(int n, Deck deck) // get rid of this?
        {
            for (; n > 0; n--)
            {
                Draw(deck);
            }
        }

        public void MakeBet(int bet)
        {
            Bet = bet;
            Cash -= bet;
        }

        public void WinBet()
        {
            Cash += Bet * 2;
            Bet = 0;
        }
    }
}