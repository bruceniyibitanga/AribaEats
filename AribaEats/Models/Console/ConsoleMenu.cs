using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats
{
    internal class ConsoleMenu : IMenu
    {
        private readonly string _menuTitle;
        private readonly IMenuItem[] _menuItems;

        public ConsoleMenu(string menuTitle, IMenuItem[] menuItems)
        {
            this._menuTitle = menuTitle;
            this._menuItems = menuItems;
        }
        public void Display()
        {
            //Console.Clear();
            Console.WriteLine($"{_menuTitle}");

            for(var i = 0; i < _menuItems.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {_menuItems[i].text}");
            }
            Console.WriteLine($"Please enter a choice between 1 and {_menuItems.Length}:");
        }

        public void EditMenuItem(int menuLocation, IMenuItem newMenuItem)
        {
            // Get the menuItem at menuLocation
            _menuItems[menuLocation] = newMenuItem;
        }

        public IMenuItem GetSelection()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int selection) &&
                    selection >= 1 &&
                    selection <= _menuItems.Length)
                {
                    return _menuItems[selection - 1];
                }

                Console.WriteLine("Invalid selection. Please try again: ");
            }
        }
    }
}

