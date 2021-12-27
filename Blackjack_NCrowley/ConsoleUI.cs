using System;

namespace Blackjack_NCrowley
{
    // For communicating between Game.cs and the console
    internal class ConsoleUI : UI
    {
        public override void DisplayOutput(string message)
        {
            Console.WriteLine(message);
        }

        public override string GetInput()
        {
            return Console.ReadLine();
        }
    }
}