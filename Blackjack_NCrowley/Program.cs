using System;

namespace Blackjack_NCrowley
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Deck deck = new Deck();
            foreach (var c in deck.Contents)
            {
                Console.WriteLine(c.Suit);
                Console.WriteLine(c.Rank);
            }
        }
    }
}