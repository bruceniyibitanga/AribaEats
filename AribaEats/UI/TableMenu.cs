using AribaEats.Interfaces;

namespace AribaEats.UI;

public class TableMenu : IMenu
{
    private readonly string _menuTitle;
    private readonly IMenuItem[] _menuItems;
    private readonly TableConfiguration _tableConfig;

    public TableMenu(string menuTitle, IMenuItem[] menuItems, TableConfiguration tableConfig)
    {
        _menuTitle = menuTitle;
        _menuItems = menuItems;
        _tableConfig = tableConfig;
    }
    
    // Number Prefix in table - "1: " - is three characters long.
    const int NUMBER_PREFIX = 3;
    
    public void Display()
    {
        if(!string.IsNullOrWhiteSpace(_menuTitle)) Console.WriteLine($"{_menuTitle}");
        
            // Display table header
            DisplayTableHeader();

            // Display menu items with table formatting
            for (var i = 0; i < _menuItems.Length; i++)
            {
                var menuItem = _menuItems[i];
                    
                if (menuItem is ITableDisplayable tableItem)
                {
                    DisplayTableRow(i + 1, tableItem.GetTableRowData());
                }
                else
                {
                    // Fallback for non-table items (like "Return to previous menu")
                    Console.WriteLine($"{i + 1}: {menuItem.text}");
                }
            }

        Console.WriteLine($"Please enter a choice between 1 and {_menuItems.Length}:");
    }
    
    private void DisplayTableHeader()
    {
        if (_tableConfig?.Headers == null || _tableConfig?.ColumnWidths == null)
            return;
        
        var headerRow = "";
        for (int i = 0; i < _tableConfig.Headers.Length; i++)
        {
            if (i == 0)
            {
                // First column includes the number prefix
                headerRow += "   " + _tableConfig.Headers[i].PadRight(_tableConfig.ColumnWidths[i] - NUMBER_PREFIX); // Account for "1: "
            }
            else
            {
                headerRow += _tableConfig.Headers[i].PadRight(_tableConfig.ColumnWidths[i]);
            }
        }
        Console.WriteLine(headerRow);
    }
    
    private void DisplayTableRow(int index, string[] rowData)
    {
        var row = $"{index}: ";
            
        for (int i = 0; i < Math.Min(rowData.Length, _tableConfig.ColumnWidths.Length); i++)
        {
            if (i == 0)
            {
                // First column accounts for the number prefix
                var availableWidth = _tableConfig.ColumnWidths[i] - NUMBER_PREFIX; // Account for "1: "
                row += (rowData[i] ?? "").PadRight(availableWidth);
            }
            else
            {
                row += (rowData[i] ?? "").PadRight(_tableConfig.ColumnWidths[i]);
            }
        }
            
        Console.WriteLine(row);
    }

    public void EditMenuItem(int menuLocation, IMenuItem newMenuItem)
    {
        _menuItems[menuLocation] = newMenuItem;
    }

    public IMenuItem GetSelection()
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int selection) &&
                selection >= 1 &&
                selection <= _menuItems.Length)
            {
                return _menuItems[selection - 1];
            }

            Console.WriteLine("Invalid selection. Please try again: ");
        }
    }
}