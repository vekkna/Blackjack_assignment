using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    internal class Game
    {
        public List<Player> Players;
        public Player Dealer;
        private Deck deck;
        private int roundsRemaining = 5;

        public Game(List<Player> players)
        {
            Players = players;
            Dealer = new Player("Dealer", 10000);
            deck = new Deck();
        }

        public void PlayRound()
        {
            Console.WriteLine($"Starting round - {roundsRemaining--} rounds remaining.");

            Players.ForEach(p => TakeBet(p));

            Dealer.Draw(2, deck);
            // TODO check if dealer has 21
            Console.WriteLine($"Dealer's face-up card is {Dealer.Hand.Cards[0]}.");

            Players.ForEach(p => p.Draw(2, deck));
            Players.ForEach(p => Console.WriteLine($"{p.Name} has {p.Hand} and is at {p.Hand.Value}."));

            Players.ForEach(p => TakeTurn(p));

            TakeDealerTurn();

            if (roundsRemaining > 0)
            {
                PlayRound();
            }
            else
            {
                Player winner = Players.OrderBy(p => p.Hand.Value).First();// TOOD replace with score board
            }
        }

        public void TakeBet(Player player)
        {
            Console.WriteLine($"{player.Name}, how much will you bet? (max: {player.Cash:C}.");
            int bet = 0;
            while (bet == 0)
            {
                try
                {
                    bet = Convert.ToInt32(Console.ReadLine());
                    if (bet < 1 || bet > player.Cash)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Console.WriteLine($"Enter a number between 1 and {player.Cash:C}.");
                    continue;
                }
            }
            Console.WriteLine($"{player.Name} makes a bet of {bet:C} and has {player.Cash:C} left");
            player.MakeBet(bet);
        }

        public void TakeTurn(Player player)
        {
            Console.WriteLine($"{player.Name}'s turn.");

            while (OfferCard(player))
            {
                Card card = player.Draw(deck);
                Console.WriteLine($"{player.Name} twists and draws a {card}. He's now on {player.Hand.Value}");
                if (player.IsBust)
                {
                    if (player.Cash == 0)
                    {
                        Console.WriteLine($"{player.Name} is bust and broke, and so is out of the game.");
                        Players.Remove(player);
                    }
                    else
                    {
                        Console.WriteLine($"{player.Name} is bust and loses {player.Bet:C}. {player.Cash:C} remaining.");
                    }
                    return;
                }
            }
            Console.WriteLine($"{player.Name} sticks at {player.Hand.Value}");
        }

        private void TakeDealerTurn()
        {
            Console.WriteLine("Dealer's turn.");
            while (Dealer.Hand.Value < 17)
            {
                Card card = Dealer.Draw(deck);
                Console.WriteLine($"Dealer draws {card} and is at {Dealer.Hand.Value}");
            }

            var activePlayers = (from p in Players where !p.IsBust select p).ToList();

            if (Dealer.IsBust)
            {
                Console.WriteLine("Dealer is bust!");
                activePlayers.ForEach(p => p.WinBet());
            }
            else
            {
                activePlayers.ForEach(p =>
                {
                    if (p.Hand.Value > Dealer.Hand.Value)
                    {
                        Console.WriteLine($"{p.Name} wins {p.Bet:C * 2} and now has {p.Cash:C}.");
                        p.WinBet();
                    }
                    else
                    {
                        Console.WriteLine($"{p.Name} loses {p.Bet:C} and now has {p.Cash:C}.");
                    }
                });
            }
        }

        public bool OfferCard(Player player)
        {
            Console.WriteLine($"{player.Name}: stick or twist? (s/t)");
            while (true)
            {
                string ans = Console.ReadLine();
                if (ans == "t")
                {
                    return true;
                }
                if (ans == "s")
                {
                    return false;
                }
                Console.WriteLine("Please answer s for stick or t for twist.");
                continue;
            }
        }
    }
}