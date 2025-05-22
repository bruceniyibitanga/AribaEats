using AribaEats.Interfaces;

namespace AribaEats.UI;

public class TableMenuItem : IMenuItem, ITableDisplayable
{
    private readonly IMenuItem _innerMenuItem;
    private readonly string[] _tableData;
    public string text => _innerMenuItem.text;

    public TableMenuItem(IMenuItem innerMenuItem, string[] tableData)
    {
        _innerMenuItem = innerMenuItem;
        _tableData = tableData;
    }
    public void Execute()
    {
        _innerMenuItem.Execute();
    }

    public string[] GetTableRowData()
    {
        return _tableData;
    }
}