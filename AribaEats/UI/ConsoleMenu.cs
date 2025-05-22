using AribaEats.Interfaces;

namespace AribaEats.UI
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
            if (!string.IsNullOrWhiteSpace(_menuTitle)) Console.WriteLine($"{_menuTitle}");

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

