using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models.Console
{
    public class MenuNavigator
    {
        private IMenu _currentMenu;
        private Stack<IMenu> _menuHistory = new Stack<IMenu>();

        public MenuNavigator(IMenu currentMenu)
        {
            _currentMenu = currentMenu;
        }

        public void SetCurrentMenu(IMenu menu)
        {
            _currentMenu = menu;
        }

        public void NavigateTo(IMenu menu)
        {
            _menuHistory.Push(_currentMenu);
            _currentMenu = menu;
        }

        public void NavigateBack()
        {
            if (_menuHistory.Count > 0)
            {
                _currentMenu = _menuHistory.Pop();
            }
        }

        public void Clear()
        {
            _menuHistory.Clear();
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
}
