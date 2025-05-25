using AribaEats.Interfaces;

namespace AribaEats.UI
{
    /// <summary>
    /// Represents a menu item that performs a specific action when selected.
    /// </summary>
    public class ActionMenuItem : IMenuItem
    {
        private readonly Action _action; // The action to execute when this menu item is selected.
        
        /// <summary>
        /// The text displayed for this menu item.
        /// </summary>
        public string text { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionMenuItem"/> class with specified text and action.
        /// </summary>
        /// <param name="text">The label or name of the menu item displayed to the user.</param>
        /// <param name="action">The action to execute when the menu item is selected. The Action
        /// represents a method that does not return anything () => {}.
        /// </param>
        public ActionMenuItem(string text, Action action)
        {
            this.text = text;
            _action = action;
        }
        
        /// <summary>
        /// Executes the action associated with this menu item.
        /// </summary>
        public void Execute()
        {
            _action();
        }
    }
}