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
        // Chosen _ui controller
        private readonly UI _ui;

        // The currently (not broke) players
        private readonly List<Player> _players;

        // Saved at start and used at end for printing the final scores
        private readonly List<Player> _startingPlayers;

        private readonly Player _dealer;
        private readonly Deck _deck;
        private int _roundsRemaining = 5;

        /// <summary>
        /// Ctor, none others needed
        /// </summary>
        /// <param name="players">List of players</param>
        /// /// <param name="ui">Chosen _ui controller</param>
        public Game(List<Player> players, UI ui)
        {
            _players = players;
            _ui = ui;
            _startingPlayers = new List<Player>(_players);
            _dealer = new Player("Dealer");
            _deck = new Deck();
        }

        /// <summary>
        /// Runs a round
        /// </summary>
        public void PlayRound()
        {
            _ui.DisplayOutput($"\nStarting round {6 - _roundsRemaining--} of 5.\n");

            RestockDeckWithDiscards();

            // Each player and the dealer draws a card before bets. Not standard BJ for the players to do so, I think, but might make bets more fun.
            AllDrawOneCard();

            // Having seen one card, and dealer's one, players make bets.
            PlayersBet();

            // _dealer draws another, if he gets 21 he wins the round...
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
        /// Discard any cards from previous round and restock the _deck with them. Not standard BJ rules, but card counting might be fun with fake money.
        /// </summary>
        private void RestockDeckWithDiscards()
        {
            var discards = (from player in _players
                            from card in player.DiscardHand()
                            select card).
                         ToList().Concat(_dealer.DiscardHand());
            _deck.Restock(discards);
        }

        /// <summary>
        /// _players and dealer draw card each
        /// </summary>
        private void AllDrawOneCard()
        {
            foreach (var player in _players)
            {
                Draw(player);
            }
            Draw(_dealer);
        }

        private void PlayersBet()
        {
            foreach (var player in _players)
            {
                TakeBet(player);
            }
        }

        /// <summary>
        /// _dealer draws second, hidden, card. If he has 21, he wins the round.
        /// </summary>
        private void DealerDrawsSecondCard()
        {
            Draw(_dealer, reveal: false);
            if (_dealer.HandValue == 21)
            {
                _ui.DisplayOutput("\nHe gets a blackjack!\n");
                foreach (var player in _players)
                {
                    PrintThatPlayerLoses(player);
                }
                if (GameShouldEnd())
                {
                    ShowFinalScores();
                }
                else
                {
                    PlayRound();
                }
            }
        }

        private void PlayersTakeTurnEach()
        {
            foreach (var player in _players)
            {
                TakeTurn(player);
            }
        }

        private void DealerTakesTurn()
        {
            // If the players are not all bust, the dealer takes a turn.
            if (_players.Any(p => !p.IsBust))
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
            Card card = _deck.TakeTopCard();
            player.AddCardToHand(card);
            _ui.DisplayOutput($"{player.Name} draws {(reveal ? card : "a hidden card.")}");
        }

        /// <summary>
        /// Asks a player to make a bet, checks for good input and stores the bet details on the player
        /// </summary>
        /// <param name="player">Player making the bet</param>
        private void TakeBet(Player player)
        {
            _ui.DisplayOutput($"\n{player.Name}, how much will you bet? (max {player.Cash:C0}).");
            // Create an input verifier that uses the passed controller to get good input, then calls an anonymous function that makes the bet
            var inputVerifier = new InputVerifier(_ui);
            inputVerifier.GetNumberInRangeThen(1, player.Cash, $"Enter a number between 1 and {player.Cash}.\n", bet =>
            {
                player.MakeBet(bet);
                _ui.DisplayOutput($"\n{player.Name} makes a bet of {bet:C0} and has {player.Cash:C0} left.");
            });
        }

        /// <summary>
        /// A player's turn. Keeps offering cards till he passes, checking for bust each time.
        /// </summary>
        /// <param name="player"></param>
        private void TakeTurn(Player player)
        {
            _ui.DisplayOutput($"{player.Name}'s turn:\n");

            // Returns true on "t(wist)", false on "s(tick)"
            while (OfferCard(player))
            {
                Draw(player);

                // If he's bust he loses his bet
                if (player.IsBust)
                {
                    _ui.DisplayOutput($"{player.Name} is bust!\n");
                    PrintThatPlayerLoses(player);

                    // and doesn't get another offer
                    return;
                }
            }
            // here player has stuck
            _ui.DisplayOutput($"\n{player.Name} sticks at {player.HandValue}.\n");
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
                _ui.DisplayOutput($"{player.Name}, you're holding {player.Hand}, so you are at {player.HandValue}.\nStick (s) or twist (t)?");
                string ans = _ui.GetInput();
                if (ans == "t")
                {
                    return true;
                }
                if (ans == "s")
                {
                    return false;
                }
                // Keep repeating till he gets it right
                _ui.DisplayOutput("Please answer s for stick or t for twist.\n");
            }
        }

        /// <summary>
        /// _dealer's turn. Must twist if lower than 17, otherwise stick
        /// </summary>
        private void TakeDealerTurn()
        {
            _ui.DisplayOutput("Dealer's turn.\n");
            _ui.DisplayOutput($"Dealer has {_dealer.Hand} and is at {_dealer.HandValue}.");

            // Draw while under 17
            while (_dealer.HandValue < 17)
            {
                Draw(_dealer);
            }

            if (_dealer.IsBust)
            {
                _ui.DisplayOutput("Dealer is bust!\n");

                foreach (var player in _players.Where(p => !p.IsBust))
                {
                    player.WinBet();
                    _ui.DisplayOutput($"{player.Name} wins {player.Bet:C0} and now has {player.Cash:C0}.\n");
                }
            }
            // If the dealer isn't bust...
            else
            {
                _ui.DisplayOutput($"Dealer sticks at {_dealer.HandValue}.\n");

                //..., compare his score to that of players who aren't bust. _dealer wins if tied.
                foreach (var player in _players.Where(p => !p.IsBust))
                {
                    if (player.HandValue > _dealer.HandValue)
                    {
                        player.WinBet();
                        _ui.DisplayOutput($"{player.Name} wins {player.Bet:C0} and now has {player.Cash:C0}.\n");
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
            _ui.DisplayOutput($"{player.Name} loses {player.Bet:C0} and now has {player.Cash:C0}.\n");
            if (player.Cash == 0)
            {
                // Actual removal is done separately to avoid removing elements from _players while working with it
                _ui.DisplayOutput($"{player.Name} is broke and out of the game!\n");
            }
        }

        /// <summary>
        /// Determines if game should end (all players are broke or 5 rounds are up)
        /// </summary>
        /// <returns>True if game should end, otherwise false</returns>
        private bool GameShouldEnd()
        {
            // Remove any broke players from the game
            _players.RemoveAll(p => p.Cash == 0);
            // Game should end if all players are broke or 5 rounds are up.
            return _players.Count == 0 || _roundsRemaining == 0;
        }

        /// <summary>
        /// Print out the final scores
        /// </summary>
        private void ShowFinalScores()
        {
            _ui.DisplayOutput("\nGame Over!\n\nFinal Scores:\n");
            // Print out final cash from greatest to least
            foreach (var player in _startingPlayers.OrderBy(p => p.Cash))
            {
                _ui.DisplayOutput($"\n{player.Name} : {player.Cash}");
            }
        }
    }
}