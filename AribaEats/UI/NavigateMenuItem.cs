using AribaEats.Interfaces;

namespace AribaEats.UI
{
    /// <summary>
    /// Represents a menu item that navigates to a different menu/screen when selected.
    /// </summary>

    public class NavigateMenuItem : IMenuItem
    {
        private readonly IMenu _targetMenu;
        private readonly MenuNavigator _navigator;

        /// <summary>
        /// The text displayed for menu item.
        /// </summary>

        public string text { get; }
        /// <summary>
        /// Creates a new menu item that allows navigation to another menu.
        /// </summary>
        /// <param name="text">The label or name of the menu item to display.</param>
        /// <param name="targetMenu">The menu to navigate to when this item is selected.</param>
        /// <param name="navigator">The navigation handler responsible for managing transitions between menus.</param>

        public NavigateMenuItem(string text, IMenu targetMenu, MenuNavigator navigator)
        {
            this.text = text;
            _targetMenu = targetMenu;
            _navigator = navigator;
        }

        /// <summary>
        /// Executes the action for this menu item by triggering navigation to the target menu.
        /// </summary>

        public void Execute()
        {
            _navigator.NavigateTo(_targetMenu);
        }
    }
}
