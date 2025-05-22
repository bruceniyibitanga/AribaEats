using AribaEats.Interfaces;

public class MenuNavigator
{
    private IMenu _currentMenu;
    private Stack<IMenu> _menuHistory = new Stack<IMenu>();

    private IMenu? _anchorMenu;
    private readonly Dictionary<string, IMenu> _homeMenus = new(); // key = user role

    public MenuNavigator(IMenu startMenu)
    {
        _currentMenu = startMenu;
    }

    public void SetCurrentMenu(IMenu menu) => _currentMenu = menu;

    public void NavigateTo(IMenu menu)
    {
        _menuHistory.Push(_currentMenu);
        _currentMenu = menu;
    }
    
    // New overload: Navigate to and optionally set anchor
    public void NavigateTo(IMenu menu, bool setAsAnchor)
    {
        NavigateTo(menu);
        if (setAsAnchor) SetAnchor();
    }

    public void NavigateBack()
    {
        if (_menuHistory.Count > 0)
        {
            _currentMenu = _menuHistory.Pop();
        }
    }

    public void SetAnchor()
    {
        _anchorMenu = _currentMenu;
    }

    public void NavigateToAnchor()
    {
        if (_anchorMenu == null || _currentMenu == _anchorMenu) return;

        // Walk back until anchor is found
        var tempStack = new Stack<IMenu>();

        while (_menuHistory.Count > 0)
        {
            var previous = _menuHistory.Pop();
            tempStack.Push(previous);

            if (previous == _anchorMenu)
            {
                _currentMenu = previous;
                break;
            }
        }

        // Restore remaining history (beyond anchor)
        while (tempStack.Count > 0)
        {
            _menuHistory.Push(tempStack.Pop());
        }
    }

    public void Clear() => _menuHistory.Clear();

    public void SetHomeMenu(string role, IMenu menu)
    {
        _homeMenus[role] = menu;
    }

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