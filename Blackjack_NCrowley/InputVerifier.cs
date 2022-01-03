using System;

namespace Blackjack_NCrowley
{
    /// <summary>
    /// Class of methods used to make sure player input is valid
    /// </summary>
    internal class InputVerifier
    {
        // Controller for chosen view
        private readonly UI _ui;

        public InputVerifier(UI ui)
        {
            this._ui = ui;
        }

        /// <summary>
        /// Makes sure input is a number in a certain range, then calls a function
        /// </summary>
        /// <param name="min">Minimum of range</param>
        /// <param name="max">Maximum of range</param>
        /// <param name="message">Instructions for valid input</param>
        /// <param name="result">Function to call when input is valid</param>
        public void GetNumberInRangeThen(int min, int max, string message, Action<int> result)
        {
            // keep asking till input is good
            while (true)
            {
                _ui.DisplayOutput(message);
                // Get input and make sure it's a number
                if (int.TryParse(_ui.GetInput(), out int number))
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

        private bool IsInRange(int number, int min, int max)
        {
            return number >= min && number <= max;
        }
    }
}