using AribaEats.Interfaces;

namespace AribaEats.UI;

/// <summary>
/// Represents a menu item that can be displayed in a tabular format while maintaining normal menu item functionality.
/// This class implements both IMenuItem for menu navigation and ITableDisplayable for table-based display.
/// </summary>
public class TableMenuItem : IMenuItem, ITableDisplayable
{
    /// <summary>
    /// The wrapped menu item that provides core menu functionality.
    /// This enables the table display capabilities to be added to any existing menu item.
    /// </summary>
    private readonly IMenuItem _innerMenuItem;

    /// <summary>
    /// Stores the data to be displayed in table columns when this item
    /// is shown in a table format (e.g., columns like Order, Restaurant Name, Dist, etc.). This represents
    /// the actual rows of data in the table
    /// </summary>
    private readonly string[] _tableData;

    /// <summary>
    /// Gets the display text for the menu item, delegating to the inner menu item.
    /// This maintains consistent text display across both table and standard menu formats.
    /// </summary>
    public string text => _innerMenuItem.text;

    /// <summary>
    /// Initializes a new instance of TableMenuItem, combining regular menu item behavior
    /// with table display capabilities.
    /// </summary>
    /// <param name="innerMenuItem">The original menu item being decorated with table functionality</param>
    /// <param name="tableData">Array of strings representing the data to be displayed in table columns</param>
    public TableMenuItem(IMenuItem innerMenuItem, string[] tableData)
    {
        _innerMenuItem = innerMenuItem;
        _tableData = tableData;
    }

    /// <summary>
    /// Executes the menu item's action using the base menu item's implementation.
    /// </summary>

    public void Execute()
    {
        _innerMenuItem.Execute();
    }

    /// <summary>
    /// Implements ITableDisplayable interface by providing the data to be shown
    /// in table format. This allows the menu item to be displayed in both
    /// standard menu lists and in tabular layouts.
    /// </summary>
    /// <returns>Array of strings containing the data for each table column</returns>
    public string[] GetTableRowData()
    {
        return _tableData;
    }
}