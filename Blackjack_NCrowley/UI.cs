using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_NCrowley
{
    internal abstract class UI
    {
        public abstract string GetInput();

        public abstract void DisplayOutput(string message);
    }
}