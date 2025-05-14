using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Interfaces
{
    /// <summary>
    /// This code defines a simple interfaces that serves as a contract for menu items in the console navigation system
    /// </summary>
    public interface IMenuItem
    {
        string text { get;  }

        /// <summary>
        /// This method defines what happens when the menu item is selected/activated.
        /// The implementation could navigate to another menu, perform an action.
        /// </summary>
        void Execute();
    }
}
