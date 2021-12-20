using System;
using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    /// <summary>
    /// Creates the game objects and runs the game.
    /// </summary>
    internal class Game
    {
        // Properties

        private readonly List<Player> Players;
        private readonly List<Player> StartingPlayers;
        private readonly Player Dealer;
        private readonly Deck deck;
        private int roundsRemaining = 5;

        /// <summary>
        /// Ctor, none others needed
        /// </summary>
        /// <param name="players">List of players</param>
        public Game(List<Player> players)
        {
            Players = players;
            StartingPlayers = new List<Player>(Players);
            Dealer = new Player("Dealer");
            deck = new Deck();
        }

        /// <summary>
        /// Runs a round
        /// </summary>
        public void PlayRound()
        {
            Console.WriteLine($"\nStarting round {6 - roundsRemaining} of 5.\n");

            // Discard any cards from previous round and restock the deck with them. Not in standard BJ rules, but card counting might be fun with fake money.
            var discards = (from player in Players
                            from card in player.Hand.Discard()
                            select card).
                            ToList().Concat(Dealer.Hand.Discard());

            deck.Restock(discards);

            // Each player and the dealer draws a card before bets. Not standard BJ for the players to do so, I think, but might make bets more fun.
            Players.ForEach(p => p.Draw(deck));
            Players.ForEach(p => Console.WriteLine($"{p.Name}'s first card is {p.Hand.Cards[0]}."));

            Dealer.Draw(deck);
            Console.WriteLine($"Dealer's first card is {Dealer.Hand.Cards[0]}.\n");

            // Having seen one card, and dealer's one, players make bets.
            Players.ForEach(p => TakeBet(p));

            // Dealer draws second card. If he has 21, he wins the round.
            Dealer.Draw(deck);
            if (Dealer.Hand.Value == 21)
            {
                Console.WriteLine($"Dealer's second card is {Dealer.Hand.Cards[1]}. He gets a blackjack!\n");
                Players.ForEach(p => PlayerLoses(p));
                if (GameShouldEnd())
                {
                    ShowFinalScores();
                    return;
                }
                else
                {
                    PlayRound();
                }
            }
            else
            {
                // If he doesn't have blackjack the round continues
                Console.WriteLine("Dealer draws a second card, which remains hidden for now.\n");
            }

            // Each player takes a turn.
            Players.ForEach(p => TakeTurn(p));

            // If the remaining players are not all bust, the dealer takes a turn.
            if (Players.Where(p => !p.IsBust).Any())
            {
                TakeDealerTurn();
            }
            // Play another round if needed
            if (GameShouldEnd())
            {
                ShowFinalScores();
                return;
            }
            else
            {
                PlayRound();
            }
        }

        /// <summary>
        /// Determines if game should end
        /// </summary>
        /// <returns>True is game should end, otherwise false</returns>
        private bool GameShouldEnd()
        {
            // Remove any broke players from the game
            Players.RemoveAll(p => p.Cash == 0);
            // Game should end if all players are broke or 5 rounds are up.
            return Players.Count() == 0 || roundsRemaining == 0;
        }

        /// <summary>
        /// Print out the final scores
        /// </summary>
        private void ShowFinalScores()
        {
            Console.WriteLine($"\nGame Over!\n\n" +
                    $"Final Scores:\n");
            // Print out final cash from greatest to least
            foreach (Player player in StartingPlayers.OrderBy(p => p.Cash))
            {
                Console.WriteLine($"{player.Name} : {player.Cash}");
            }
        }

        /// <summary>
        /// Asks a player to make a bet, checks for good input and stores the bet details
        /// </summary>
        /// <param name="player">Player making the bet</param>
        private void TakeBet(Player player)
        {
            Console.WriteLine($"{player.Name}, how much will you bet? (max: {player.Cash:C0}).");
            int bet;
            // Check for valid input
            while (true)
            {
                // if a number isn't entered,error is thrown
                try
                {
                    bet = Convert.ToInt32(Console.ReadLine());
                    // if number is out of range, error is thrown
                    if (bet < 1 || bet > player.Cash)
                    {
                        throw new Exception();
                    }
                    // if input is a number in the right range, break out of of the validation loop
                    break;
                }
                catch
                {
                    // if either error is commited, remind player of valid input and try again
                    Console.WriteLine($"Enter a number between 1 and {player.Cash:C0}.\n");
                    continue;
                }
            }
            // Once input is good, make the bet
            player.MakeBet(bet);
            Console.WriteLine($"\n{player.Name} makes a bet of {bet:C0} and has {player.Cash:C0} left.\n");
        }

        /// <summary>
        /// A player's turn. Keeps offering cards till he passes, checking for bust each time.
        /// </summary>
        /// <param name="player"></param>
        private void TakeTurn(Player player)
        {
            Console.WriteLine($"{player.Name}'s turn:\n");

            // Returns true on "t(wist)", false on "s(tick)"
            while (OfferCard(player))
            {
                // Player draws a card
                Card card = player.Draw(deck);
                Console.WriteLine($"\n{player.Name} draws {card}.\n");

                // If he's bust
                if (player.IsBust)
                {
                    Console.WriteLine($"{player.Name} is bust!\n");
                    PlayerLoses(player);

                    // don't offer another card if he's bust
                    return;
                }
            }
            // here player has stuck
            Console.WriteLine($"\n{player.Name} sticks at {player.Hand.Value}.\n");
        }

        /// <summary>
        /// Dealer's turn. Must twist if lower than 17, otherwuse stick
        /// </summary>
        private void TakeDealerTurn()
        {
            Console.WriteLine("Dealer's turn.\n");
            Console.WriteLine($"Dealer has {Dealer.Hand} and is at {Dealer.Hand.Value}.");
            // Draw while under 17
            while (Dealer.Hand.Value < 17)
            {
                Card card = Dealer.Draw(deck);
                Console.WriteLine($"Dealer draws {card} and is at {Dealer.Hand.Value}.\n");
            }

            if (Dealer.IsBust)
            {
                Console.WriteLine("Dealer is bust!\n");

                DealerLoses();
            }
            // If the dealer isn't bust...
            else
            {
                Console.WriteLine($"Dealer sticks at {Dealer.Hand.Value}.\n");

                //..., compare his score to that of those players who aren't bust.
                foreach (var player in Players.Where(p => !p.IsBust))
                {
                    if (player.Hand.Value > Dealer.Hand.Value)
                    {
                        DealerLoses();
                    }
                    else
                    {
                        PlayerLoses(player);
                    }
                }
            }
        }

        /// <summary>
        /// When dealer loses, non-bust players win their bets
        /// </summary>
        private void DealerLoses()
        {
            foreach (var player in Players.Where(p => !p.IsBust))
            {
                player.WinBet();
                Console.WriteLine($"{player.Name} wins {player.Bet:C0} and now has {player.Cash:C0}.\n");
            }
        }

        /// <summary>
        /// Just comminicates the face - nothing else needed as players already paid their bets
        /// </summary>
        /// <param name="player">Losing player</param>
        private void PlayerLoses(Player player)
        {
            Console.WriteLine($"{player.Name} loses {player.Bet:C0} and now has {player.Cash:C0}.\n");
            if (player.Cash == 0)
            {
                Console.WriteLine($"{player.Name} is broke and out of the game!\n");
            }
        }

        /// <summary>
        ///  Ask player if he's like another card
        /// </summary>
        /// <param name="player">Player asked</param>
        /// <returns>True if another card is wanted, otherwise false</returns>
        private bool OfferCard(Player player)
        {
            while (true)
            {
                Console.WriteLine($"{player.Name}, you're holding {player.Hand} and are at {player.Hand.Value}.\n" +
"Stick (s) or twist (t)?");
                string ans = Console.ReadLine();
                if (ans == "t")
                {
                    return true;
                }
                if (ans == "s")
                {
                    return false;
                }
                Console.WriteLine("Please answer s for stick or t for twist.\n");
                continue;
            }
        }
    }
}