using System.Collections.Generic;

namespace Blackjack_NCrowley
{
    /// <summary>
    /// The players and the dealer
    /// </summary>
    internal class Player
    {
        // Properties
        ///
        public string Name { get; }

        public int Cash { get; private set; }
        public int Bet { get; private set; }
        public Hand Hand { get; }
        public int HandValue => Hand.Value;
        public bool IsBust => Hand.Value > 21;

        /// <summary>
        /// Constructor that takes an int and turns it into a Name string
        /// </summary>
        /// <param name="num">Int, converted to string number word</param>
        /// <param name="cash">Default value for starting cash is 100</param>
        public Player(int num, int cash = 100)
        {
            // To convert the number entered as number of players into a number word for the Name
            var numberWords = new Dictionary<int, string>()
        {
            {1, "One" },
            {2, "Two" },
            {3, "Three" },
            {4, "Four" }
        };
            Name = $"Player { numberWords[num]}";
            Cash = cash;
            Hand = new Hand();
        }

        /// <summary>
        /// Ctor for making a player with a string for a name
        /// </summary>
        /// <param name="name">Player's name</param>
        /// <param name="cash">Default value for starting cash is 100</param>
        public Player(string name, int cash = 100)
        {
            Name = name;
            Cash = cash;
            Hand = new Hand();
        }

        public List<Card> DiscardHand()
        {
            return Hand.Discard();
        }

        public void AddCardToHand(Card card)
        {
            Hand.AddCard(card);
        }

        /// <summary>
        /// Makes a bet, decreasing cash by that amount
        /// </summary>
        /// <param name="bet">Amount to bet</param>
        public void MakeBet(int bet)
        {
            Bet = bet;
            // Pay for bet now so that losses don't need to be handled later
            Cash -= bet;
        }

        /// <summary>
        /// Player wins bet
        /// </summary>
        public void WinBet()
        {
            //times 2 to make up for the outlay
            Cash += Bet * 2;
        }
    }
}