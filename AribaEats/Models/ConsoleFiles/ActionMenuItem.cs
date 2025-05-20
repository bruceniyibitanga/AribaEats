using AribaEats.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Models.ConsoleFiles
{
    public class ActionMenuItem : IMenuItem
    {
        private readonly Action _action;
        public string text { get; }

        public ActionMenuItem(string text, Action action)
        {
            this.text = text;
            _action = action;
            
        }
        
        public void Execute()
        {
            _action();
        }
    }
}
