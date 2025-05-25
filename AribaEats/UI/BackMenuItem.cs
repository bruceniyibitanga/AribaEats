using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.UI
{
    /// <summary>
    /// Represents a menu item that facilitates navigation back to the previous menu.
    /// </summary>
    public class BackMenuItem : IMenuItem
    {
        public string text { get; }
        private readonly MenuNavigator _navigator;

        public BackMenuItem(string text, MenuNavigator navigator)
        {
            this.text = text;
            _navigator = navigator;
        }

        public void Execute()
        {
            _navigator.NavigateBack();
        }
    }
}
