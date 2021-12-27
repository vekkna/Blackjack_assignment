namespace Blackjack_NCrowley
{
    /// <summary>
    /// Abstract class for communicating with chosen UI.
    /// Derive classes from from it for various UI options
    /// </summary>
    internal abstract class UI
    {
        public abstract string GetInput();

        public abstract void DisplayOutput(string message);
    }
}