using AribaEats.Interfaces;

namespace AribaEats.UI
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
