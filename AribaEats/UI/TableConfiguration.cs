namespace AribaEats.UI;

public class TableConfiguration
{
    public string[] Headers { get; }
    public int[] ColumnWidths { get; }

    public TableConfiguration(string[] headers, int[] columnWidths)
    {
        if (headers.Length != columnWidths.Length)
            throw new ArgumentException("Headers and column widths must have the same length");
            
        Headers = headers;
        ColumnWidths = columnWidths;
    }
}