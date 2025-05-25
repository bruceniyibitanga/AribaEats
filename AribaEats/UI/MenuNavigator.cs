using AribaEats.Interfaces;

/// <summary>
/// Handles navigation between menus in the application, managing menu transitions, history, and special navigation states like anchors and home screens.
/// </summary>

public class MenuNavigator
{
    private IMenu _currentMenu;
    
    /// <summary>
    /// As the name suggests this will hold the menu history.
    /// As the user navigates to different screen the previous
    /// screen will be added to the stack.
    /// </summary>
    private Stack<IMenu> _menuHistory = new Stack<IMenu>();

    /// <summary>
    /// This was added because there are some instances that users aren't
    /// redirected backward in order of menu history
    /// or redirected to the main menu but rather at some anchor point.
    /// Therefore, an optional but useful navigation.
    /// </summary>
    private IMenu? _anchorMenu;
    private readonly Dictionary<string, IMenu> _homeMenus = new(); // key = user role

    public MenuNavigator(IMenu startMenu)
    {
        _currentMenu = startMenu;
    }

    /// <summary>
    /// Sets the current menu without adding it to the history. Only used at the start of the application.
    /// </summary>
    /// <param name="menu">The menu to set as the current menu.</param>

    public void SetCurrentMenu(IMenu menu) => _currentMenu = menu;

    /// <summary>
    /// Navigates to the specified menu, adding the current menu to the history.
    /// </summary>
    /// <param name="menu">The menu to navigate to.</param>
    public void NavigateTo(IMenu menu)
    {
        _menuHistory.Push(_currentMenu);
        _currentMenu = menu;
    }
    
    /// <summary>
    /// Overload for NavigateTo: Navigates to the specified menu and optionally sets it as the anchor menu.
    /// </summary>
    /// <param name="menu">The menu to navigate to.</param>
    /// <param name="setAsAnchor">If true, sets the current menu as the anchor.</param>
    public void NavigateTo(IMenu menu, bool setAsAnchor)
    {
        NavigateTo(menu);
        if (setAsAnchor) SetAnchor();
    }

    /// <summary>
    /// Navigates to the previous menu in the history.
    /// </summary>
    public void NavigateBack()
    {
        if (_menuHistory.Count > 0)
        {
            _currentMenu = _menuHistory.Pop();
        }
    }

    /// <summary>
    /// Sets the current menu as the anchor menu for quick access during navigation.
    /// </summary>
    public void SetAnchor()
    {
        _anchorMenu = _currentMenu;
    }

    /// <summary>
    /// Navigates to the previously set anchor menu, if available, restoring the history up to that point.
    /// </summary>
    public void NavigateToAnchor()
    {
        // If there is no anchor menu or we're already at the anchor, do nothing
        if (_anchorMenu == null || _currentMenu == _anchorMenu) return;

        // Create a temporary stack to store popped menus while searching for the anchor
        var tempStack = new Stack<IMenu>();

        // Loop through the menu history
        while (_menuHistory.Count > 0)
        {
            // Remove the most recent menu from history
            var previous = _menuHistory.Pop();
        
            // Store it temporarily
            tempStack.Push(previous);

            // If the anchor menu is found, set it as the current menu and stop
            if (previous == _anchorMenu)
            {
                _currentMenu = previous;
                break;
            }
        }
        
        // Put the menus (after the anchor) back into the history stack
        while (tempStack.Count > 0)
        {
            _menuHistory.Push(tempStack.Pop());
        }
    }

    
    /// <summary>
    /// Clears the entire navigation history.
    /// </summary>
    public void Clear() => _menuHistory.Clear();

    /// <summary>
    /// Sets a home menu for a specific user role.
    /// </summary>
    /// <param name="role">The user role ("customer", "client" or "deliverer").</param>
    /// <param name="menu">The menu to set as the home menu for the role.</param>
    public void SetHomeMenu(string role, IMenu menu)
    {
        _homeMenus[role] = menu;
    }
    
    /// <summary>
    /// Works similarily to the anchor navigation but rather
    /// redirects straight to the logged-in user's home page.
    /// </summary>
    /// <param name="role"></param>
    public void NavigateHome(string role)
    {
        if (_homeMenus.TryGetValue(role, out var homeMenu))
        {
            _menuHistory.Push(_currentMenu);
            _currentMenu = homeMenu;
        }
        else
        {
            Console.WriteLine("Home menu not set for role: " + role);
        }
    }
    
    /// <summary>
    /// Starts the menu navigation loop, handling user input and executing menu actions until the application ends.
    /// </summary>
    public void Start()
    {
        while (true)
        {
            _currentMenu.Display();
            var selection = _currentMenu.GetSelection();
            selection.Execute();
        }
    }
}