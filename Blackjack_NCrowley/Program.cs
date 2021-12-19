using System;
using System.Collections;
using System.Collections.Generic;

namespace Blackjack_NCrowley
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Game game = new Game(GetPlayers());
            game.PlayRound();
        }

        private static List<Player> GetPlayers()
        {
            Console.WriteLine("Welcome to Blackjack.");
            Console.WriteLine("How many human players are there? (1 - 4)");
            int numPlayers;

            while (true)
            {
                try
                {
                    numPlayers = Convert.ToInt32(Console.ReadLine());
                    if (numPlayers < 1 || numPlayers > 5)
                    {
                        throw new Exception();
                    }
                    break;
                }
                catch
                {
                    Console.WriteLine("Please enter only numbers between 1 and 4.");
                    continue;
                }
            }

            List<Player> players = new List<Player>();

            for (int i = 0; i < numPlayers; i++)
            {
                players.Add(new Player($"Player {i + 1}"));
            }

            return players;
        }
    }
}