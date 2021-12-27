using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    internal class Program
    {
        private static void Main()
        {
            // To display euro signs
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            // Create a game, with chosen players and chosen UI view
            Game game = new Game(GetPlayers(), new ConsoleUI());
            game.PlayRound();
        }

        /// <summary>
        /// Asks how many players there will be and checks validity, then returns a list of that many players
        /// </summary>
        /// <returns>List of players</returns>
        private static List<Player> GetPlayers()
        {
            Console.WriteLine("Welcome to Blackjack.\n\n");
            Console.WriteLine("How many human players are there?");
            var players = new List<Player>();
            var inputVerifier = new InputVerifier(new ConsoleUI());
            inputVerifier.GetNumberInRangeThen(1, 4, "Enter a number between 1 and 4", numPlayers =>
            {
                players = Enumerable.Range(1, numPlayers).Select(i => new Player(i)).ToList();
            });
            return players;
        }
    }
}