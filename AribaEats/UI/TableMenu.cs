using AribaEats.Interfaces;

namespace AribaEats.UI;

/// <summary>
/// Implements a menu that displays items in a formatted table structure.
/// Supports both standard menu items and items with additional tabular data.
/// </summary>
public class TableMenu : IMenu
{
    private readonly string _menuTitle;
    private readonly IMenuItem[] _menuItems;
    private readonly TableConfiguration _tableConfig;
    private readonly bool _showRowNumbers = true;

    /// <summary>
    /// Creates a new table-formatted menu.
    /// </summary>
    /// <param name="menuTitle">Title displayed at the top of the menu</param>
    /// <param name="menuItems">Collection of items to display in the menu</param>
    /// <param name="tableConfig">Configuration for table header titles and column widths/padding</param>
    /// <param name="showRowNumbers">Whether to display row numbers for menu selection</param>
    public TableMenu(string menuTitle, IMenuItem[] menuItems, TableConfiguration tableConfig, bool showRowNumbers = true)
    {
        _menuTitle = menuTitle;
        _menuItems = menuItems;
        _tableConfig = tableConfig;
        _showRowNumbers = showRowNumbers;
    }
    
    // Number Prefix in table - "1: " - is three characters long.
    const int NUMBER_PREFIX = 3;
    
    /// <summary>
    /// Renders the menu to the console, showing items in a table format.
    /// Handles both regular menu items and those with table data.
    /// </summary>
    public void Display()
    {
        if(!string.IsNullOrWhiteSpace(_menuTitle)) Console.WriteLine($"{_menuTitle}");
        
        DisplayTableHeader();

        for (var i = 0; i < _menuItems.Length; i++)
        {
            var menuItem = _menuItems[i];
                    
            if (menuItem is ITableDisplayable tableItem)
            {
                DisplayTableRow(i + 1, tableItem.GetTableRowData());
            }
            else
            {
                Console.WriteLine(_showRowNumbers ? $"{i + 1}: {menuItem.text}" : menuItem.text);                
            }
        }

        Console.WriteLine($"Please enter a choice between 1 and {_menuItems.Length}:");
    }
    
    /// <summary>
    /// Displays the table header row with proper column spacing.
    /// Accounts for the number prefix in the first column's width.
    /// </summary>
    private void DisplayTableHeader()
    {
        if (_tableConfig?.Headers == null || _tableConfig?.ColumnWidths == null)
            return;
        
        var headerRow = "";
        for (int i = 0; i < _tableConfig.Headers.Length; i++)
        {
            if (i == 0)
            {
                headerRow += "   " + _tableConfig.Headers[i].PadRight(_tableConfig.ColumnWidths[i] - NUMBER_PREFIX);
            }
            else
            {
                headerRow += _tableConfig.Headers[i].PadRight(_tableConfig.ColumnWidths[i]);
            }
        }
        Console.WriteLine(headerRow);
    }
    
    /// <summary>
    /// Displays a single row of the table with proper formatting.
    /// Ensures consistent column widths and handles the row number prefix.
    /// </summary>
    /// <param name="index">Row number to display</param>
    /// <param name="rowData">Data to display in each column</param>
    private void DisplayTableRow(int index, string[] rowData)
    {
        var row = $"{index}: ";
            
        for (int i = 0; i < Math.Min(rowData.Length, _tableConfig.ColumnWidths.Length); i++)
        {
            if (i == 0)
            {
                var availableWidth = _tableConfig.ColumnWidths[i] - NUMBER_PREFIX;
                row += (rowData[i] ?? "").PadRight(availableWidth);
            }
            else
            {
                row += (rowData[i] ?? "").PadRight(_tableConfig.ColumnWidths[i]);
            }
        }
            
        Console.WriteLine(row);
    }

    /// <summary>
    /// Updates a menu item at the specified position. Used in the case to edit a particular menu item row.
    /// </summary>
    /// <param name="menuLocation">Index of the menu item to update</param>
    /// <param name="newMenuItem">New menu item to insert at the specified position</param>
    public void EditMenuItem(int menuLocation, IMenuItem newMenuItem)
    {
        _menuItems[menuLocation] = newMenuItem;
    }

    /// <summary>
    /// Gets the user's menu selection, ensuring it's valid.
    /// Continues prompting until a valid selection is made.
    /// </summary>
    /// <returns>The selected menu item</returns>
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