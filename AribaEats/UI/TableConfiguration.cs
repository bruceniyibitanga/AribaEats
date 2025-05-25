namespace AribaEats.UI;

/// <summary>
/// This class defines the table structure or configuration (headers and column width/padding)
/// </summary>
public class TableConfiguration
{
    /// <summary>
    /// The titles of the table columns (e.g., "Order", "Restaurant Name", "Dist", etc.).
    /// </summary>
    public string[] Headers { get; }
    /// <summary>
    /// The width/padding assigned to each column in the table.
    /// </summary>

    public int[] ColumnWidths { get; }
    
    /// <summary>
    /// Creates the configuration for a table.
    /// </summary>
    /// <param name="headers">The column titles for the table.</param>
    /// <param name="columnWidths">The corresponding widths for each column.</param>
    /// <exception cref="ArgumentException">Thrown if the number of headers and column widths do not match.</exception>
    public TableConfiguration(string[] headers, int[] columnWidths)
    {
        if (headers.Length != columnWidths.Length)
            throw new ArgumentException("Headers and column widths must have the same length");
            
        Headers = headers;
        ColumnWidths = columnWidths;
    }
}