using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    /*
     * TODOs
     * Extra score for bj
     * Save high score
     * Separate Dealer.cs?
     * format cards better
     * */

    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Game game = new Game(GetPlayers(), new ConsoleUI());
            game.PlayRound();
        }

        /// <summary>
        /// Asks how many players there will be and checks validity
        /// </summary>
        /// <returns>Number of players</returns>
        private static List<Player> GetPlayers()
        {
            Console.WriteLine("Welcome to Blackjack.\n\n");
            Console.WriteLine("How many human players are there?");
            var players = new List<Player>();
            var inputVerifier = new InputVerifier(new ConsoleUI());
            inputVerifier.GetNumberInRangeThen(1, 4, "Enter a number between 1 and 4", numPlayers =>
            {
                players = (from i in Enumerable.Range(1, numPlayers) select new Player(i)).ToList();
            });
            return players;
        }

        /*            // Keep asking till he gets it right
                    while (true)
                    {
                        Console.WriteLine("Enter a number between 1 and 4");
                        // Make sure it's a number
                        if (int.TryParse(Console.ReadLine(), out int numPlayers))
                        {
                            // If it's in range, return a list of that many players for the game's ctor
                            if (numPlayers > 0 && numPlayers < 5)
                            {
                                return (from i in Enumerable.Range(1, numPlayers) select new Player(i)).ToList();
                            }
                        }
                    }*/
    }
}