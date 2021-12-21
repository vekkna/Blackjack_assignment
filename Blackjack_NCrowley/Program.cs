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
            Game game = new Game(GetPlayers());
            game.PlayRound();
        }

        /// <summary>
        /// Asks how many players there will be and checks validity
        /// </summary>
        /// <returns>Number of players</returns>
        private static List<Player> GetPlayers()
        {
            Console.WriteLine("Welcome to Blackjack.\n\n");
            Console.WriteLine("How many human players are there? (1 - 4)");

            int numPlayers;

            while (true)
            {
                // if player doesn't enter a number, throw an error
                try
                {
                    numPlayers = Convert.ToInt32(Console.ReadLine());
                    //  if it's a number but not in the range, throw an error
                    if (numPlayers < 1 || numPlayers > 4)
                    {
                        throw new Exception();
                    }
                    // if it's a number in the right range break out of the validation loop
                    break;
                }
                catch
                {
                    // respond to either error this way
                    Console.WriteLine("Please enter only numbers between 1 and 4.\n");
                }
            }
            // Once the input is correct, return a list of that many players to pass to Game.cs's ctor
            return (from i in Enumerable.Range(1, numPlayers) select new Player(i)).ToList();
        }
    }
}