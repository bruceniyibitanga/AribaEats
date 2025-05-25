using AribaEats.Interfaces;

namespace AribaEats.UI
{
    /// <summary>
    /// Represents a simple console-based menu system for selecting items from a list.
    /// </summary>
    internal class ConsoleMenu : IMenu
    {
        // Title displayed at the top of the menu.
        private readonly string _menuTitle; 
        // Array of menu items displayed to the user.
        private readonly IMenuItem[] _menuItems; 
        // Controls whether row numbers are displayed for prompts.
        private readonly bool _showRowNumbers; 
        // Controls whether the final selection prompt is displayed.
        private readonly bool _showLastPrompt; 

        /// <summary>
        /// Initializes the console menu with a title, items, and display options.
        /// </summary>
        /// <param name="menuTitle">The title of the menu displayed at the top.</param>
        /// <param name="menuItems">An array of menu items to display for user selection.</param>
        /// <param name="showRowNumbers">Indicates whether to display row numbers next to menu items (default is true).</param>
        /// <param name="showLastPrompt">Indicates whether to display the prompt for user input at the end (default is true).</param>
        public ConsoleMenu(string menuTitle, IMenuItem[] menuItems, bool showRowNumbers = true, bool showLastPrompt = true)
        {
            _menuTitle = menuTitle;
            _menuItems = menuItems;
            _showRowNumbers = showRowNumbers;
            _showLastPrompt = showLastPrompt;
        }

        /// <summary>
        /// Displays the menu items to the console, optionally including row numbers and prompts.
        /// </summary>
        public void Display()
        {
            if (!string.IsNullOrWhiteSpace(_menuTitle)) 
                Console.WriteLine($"{_menuTitle}");

            for (var i = 0; i < _menuItems.Length; i++)
            {
                Console.WriteLine(_showRowNumbers ? $"{i + 1}: {_menuItems[i].text}" : $"{_menuItems[i].text}");
            }

            if (_showLastPrompt) 
                Console.WriteLine($"Please enter a choice between 1 and {_menuItems.Length}:");
        }

        /// <summary>
        /// Updates a menu item at a specific position in the menu.
        /// </summary>
        /// <param name="menuLocation">The zero-based index of the menu item to update.</param>
        /// <param name="newMenuItem">The new menu item to replace the existing item at the specified index.</param>
        public void EditMenuItem(int menuLocation, IMenuItem newMenuItem)
        {
            _menuItems[menuLocation] = newMenuItem;
        }

        /// <summary>
        /// Reads and returns the user's menu selection, ensuring valid input is provided.
        /// Continues to prompt the user until a valid option is entered.
        /// </summary>
        /// <returns>The selected menu item based on user input.</returns>
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