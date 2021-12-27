using System.Collections.Generic;
using System.Linq;

namespace Blackjack_NCrowley
{
    /// <summary>
    /// Creates the game objects and runs the game.
    /// </summary>
    internal class Game
    {
        // Fields
        // Chosen UI controller
        private readonly UI UI;

        // The currently (not broke) players
        private readonly List<Player> Players;

        // Saved at start and used at end for printing the final scores
        private readonly List<Player> StartingPlayers;

        private readonly Player Dealer;
        private readonly Deck deck;
        private int roundsRemaining = 5;

        /// <summary>
        /// Ctor, none others needed
        /// </summary>
        /// <param name="players">List of players</param>
        /// /// <param name="ui">Chosen UI controller</param>
        public Game(List<Player> players, UI ui)
        {
            Players = players;
            UI = ui;
            StartingPlayers = new List<Player>(Players);
            Dealer = new Player("Dealer");
            deck = new Deck();
        }

        /// <summary>
        /// Runs a round
        /// </summary>
        public void PlayRound()
        {
            UI.DisplayOutput($"\nStarting round {6 - roundsRemaining--} of 5.\n");

            RestockDeckWithDiscards();

            // Each player and the dealer draws a card before bets. Not standard BJ for the players to do so, I think, but might make bets more fun.
            AllDrawOneCard();

            // Having seen one card, and dealer's one, players make bets.
            PlayersBet();

            // Dealer drawns another, if he gets 21 he wins the round...
            DealerDrawsSecondCard();

            // ...otherwise, each player takes a turn.
            PlayersTakeTurnEach();

            // If some players aren't bust, the dealer takes a turn
            DealerTakesTurn();

            // Play another round if needed
            if (GameShouldEnd())
            {
                ShowFinalScores();
                return;
            }
            PlayRound();
        }

        /// <summary>
        /// Discard any cards from previous round and restock the deck with them. Not standard BJ rules, but card counting might be fun with fake money.
        /// </summary>
        private void RestockDeckWithDiscards()
        {
            //  var discards = Players.SelectMany(player => player.Hand.Discard().Select(card => card)).ToList().Concat(Dealer.Hand.Discard());
            var discards = (from player in Players
                            from card in player.DiscardHand()
                            select card).
                         ToList().Concat(Dealer.DiscardHand());
            deck.Restock(discards);
        }

        /// <summary>
        /// Players and dealer draw card each
        /// </summary>
        private void AllDrawOneCard()
        {
            foreach (var player in Players)
            {
                Draw(player);
            }
            Draw(Dealer);
        }

        private void PlayersBet()
        {
            foreach (var player in Players)
            {
                TakeBet(player);
            }
        }

        /// <summary>
        /// Dealer draws second, hidden, card. If he has 21, he wins the round.
        /// </summary>
        private void DealerDrawsSecondCard()
        {
            Draw(Dealer, reveal: false);
            if (Dealer.HandValue == 21)
            {
                UI.DisplayOutput("\nHe gets a blackjack!\n");
                foreach (var player in Players)
                {
                    PrintThatPlayerLoses(player);
                }
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
        }

        private void PlayersTakeTurnEach()
        {
            foreach (var player in Players)
            {
                TakeTurn(player);
            }
        }

        private void DealerTakesTurn()
        {
            // If the players are not all bust, the dealer takes a turn.
            if (Players.Where(p => !p.IsBust).Any())
            {
                TakeDealerTurn();
            }
        }

        /// <summary>
        /// Draw a card,
        /// </summary>
        /// <param name="player">The player to get the card</param>
        /// <param name="reveal">Whether the others should see the card or not</param>
        private void Draw(Player player, bool reveal = true)
        {
            Card card = deck.TakeTopCard();
            player.AddCardToHand(card);
            if (reveal)
            {
                UI.DisplayOutput($"{player.Name} draws {card}.\n");
            }
            else
            {
                UI.DisplayOutput($"{player.Name} draws a hidden card.\n");
            }
        }

        /// <summary>
        /// Asks a player to make a bet, checks for good input and stores the bet details on the player
        /// </summary>
        /// <param name="player">Player making the bet</param>
        private void TakeBet(Player player)
        {
            UI.DisplayOutput($"\n{player.Name}, how much will you bet? (max {player.Cash:C0}).");
            // Create an input verifier that uses the passed controller to get good input, then calls an anonymous function that makes the bet
            var inputVerifier = new InputVerifier(UI);
            inputVerifier.GetNumberInRangeThen(1, player.Cash, $"Enter a number between 1 and {player.Cash}.\n", bet =>
            {
                player.MakeBet(bet);
                UI.DisplayOutput($"\n{player.Name} makes a bet of {bet:C0} and has {player.Cash:C0} left.");
            });
        }

        /// <summary>
        /// A player's turn. Keeps offering cards till he passes, checking for bust each time.
        /// </summary>
        /// <param name="player"></param>
        private void TakeTurn(Player player)
        {
            UI.DisplayOutput($"{player.Name}'s turn:\n");

            // Returns true on "t(wist)", false on "s(tick)"
            while (OfferCard(player))
            {
                Draw(player);

                // If he's bust he loses his bet
                if (player.IsBust)
                {
                    UI.DisplayOutput($"{player.Name} is bust!\n");
                    PrintThatPlayerLoses(player);

                    // and doesn't get another offer
                    return;
                }
            }
            // here player has stuck
            UI.DisplayOutput($"\n{player.Name} sticks at {player.HandValue}.\n");
        }

        /// <summary>
        ///  Asks player if he'd like another card
        /// </summary>
        /// <param name="player">Player asked</param>
        /// <returns>True if another card is wanted, otherwise false</returns>
        private bool OfferCard(Player player)
        {
            // To check for valid input (s/t)
            while (true)
            {
                UI.DisplayOutput($"{player.Name}, you're holding {player.Hand}, so you are at {player.HandValue}.\nStick (s) or twist (t)?");
                string ans = UI.GetInput();
                if (ans == "t")
                {
                    return true;
                }
                if (ans == "s")
                {
                    return false;
                }
                // Keep repeating till he gets it right
                UI.DisplayOutput("Please answer s for stick or t for twist.\n");
                continue;
            }
        }

        /// <summary>
        /// Dealer's turn. Must twist if lower than 17, otherwuse stick
        /// </summary>
        private void TakeDealerTurn()
        {
            UI.DisplayOutput("Dealer's turn.\n");
            UI.DisplayOutput($"Dealer has {Dealer.Hand} and is at {Dealer.HandValue}.");

            // Draw while under 17
            while (Dealer.HandValue < 17)
            {
                Draw(Dealer);
            }

            if (Dealer.IsBust)
            {
                UI.DisplayOutput("Dealer is bust!\n");

                foreach (var player in Players.Where(p => !p.IsBust))
                {
                    player.WinBet();
                    UI.DisplayOutput($"{player.Name} wins {player.Bet:C0} and now has {player.Cash:C0}.\n");
                }
            }
            // If the dealer isn't bust...
            else
            {
                UI.DisplayOutput($"Dealer sticks at {Dealer.HandValue}.\n");

                //..., compare his score to that of players who aren't bust. Dealer wins if tied.
                foreach (var player in Players.Where(p => !p.IsBust))
                {
                    if (player.HandValue > Dealer.HandValue)
                    {
                        player.WinBet();
                        UI.DisplayOutput($"{player.Name} wins {player.Bet:C0} and now has {player.Cash:C0}.\n");
                    }
                    else
                    {
                        PrintThatPlayerLoses(player);
                    }
                }
            }
        }

        /// <summary>
        /// Communicates that a player has lost his bet
        /// </summary>
        /// <param name="player">Losing player</param>
        private void PrintThatPlayerLoses(Player player)
        {
            // No need to reduce player's cash - this was done when he made the bet
            UI.DisplayOutput($"{player.Name} loses {player.Bet:C0} and now has {player.Cash:C0}.\n");
            if (player.Cash == 0)
            {
                // Actual removal is done seperately to avoid removing elements from Players while working with it
                UI.DisplayOutput($"{player.Name} is broke and out of the game!\n");
            }
        }

        /// <summary>
        /// Determines if game should end (all players are broke or 5 rounds are up)
        /// </summary>
        /// <returns>True if game should end, otherwise false</returns>
        private bool GameShouldEnd()
        {
            // Remove any broke players from the game
            Players.RemoveAll(p => p.Cash == 0);
            // Game should end if all players are broke or 5 rounds are up.
            return Players.Count == 0 || roundsRemaining == 0;
        }

        /// <summary>
        /// Print out the final scores
        /// </summary>
        private void ShowFinalScores()
        {
            UI.DisplayOutput($"\nGame Over!\n\nFinal Scores:\n");
            // Print out final cash from greatest to least
            foreach (var player in StartingPlayers.OrderBy(p => p.Cash))
            {
                UI.DisplayOutput($"\n{player.Name} : {player.Cash}");
            }
        }
    }
}