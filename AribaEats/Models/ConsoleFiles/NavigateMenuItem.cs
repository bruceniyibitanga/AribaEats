using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models.ConsoleFiles
{
    public class NavigateMenuItem : IMenuItem
    {
        private readonly IMenu _targetMenu;
        private readonly MenuNavigator _navigator;

        public string text { get; }
        public NavigateMenuItem(string text, IMenu targetMenu, MenuNavigator navigator)
        {
            this.text = text;
            this._targetMenu = targetMenu;
            this._navigator = navigator;
        }

        public void Execute()
        {
            _navigator.NavigateTo(_targetMenu);
        }
    }
}
