using System;

namespace Blackjack_NCrowley
{
    internal class InputVerifier
    {
        public static void GetNumberInRangeThen(int min, int max, string message, Action<int> result)
        {
            // keep asking till input is good
            while (true)
            {
                Console.WriteLine(message);
                // Get input and make sure it's a number
                if (int.TryParse(Console.ReadLine(), out int number))
                {
                    // If it's in range, call the result
                    if (IsInRange(number, min, max))
                    {
                        result(number);
                        return;
                    }
                }
            }
        }

        private static bool IsInRange(int number, int min, int max)
        {
            return number >= min && number <= max;
        }
    }
}