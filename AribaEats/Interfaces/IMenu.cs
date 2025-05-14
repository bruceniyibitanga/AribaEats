using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AribaEats.Interfaces
{
    /// <summary>
    /// This iterface defines how the menu screen should be implemented
    /// </summary>
    public interface IMenu
    {
        void Display();
        void EditMenuItem(int menuLocation, IMenuItem newMenuItem);
        IMenuItem GetSelection();
    }
}
