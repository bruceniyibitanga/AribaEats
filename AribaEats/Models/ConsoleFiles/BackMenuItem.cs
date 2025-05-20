using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AribaEats.Models.ConsoleFiles;

namespace AribaEats.Models.ConsoleFiles
{
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
