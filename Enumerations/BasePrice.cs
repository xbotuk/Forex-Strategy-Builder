namespace Forex_Strategy_Builder
{
    /// <summary>
    /// The type of base price
    /// </summary>
    public enum BasePrice
    {
        Open,
        High,
        Low,
        Close,
        Median,  // Price[bar] = (Low[bar] + High[bar]) / 2;
        Typical, // Price[bar] = (Low[bar] + High[bar] + Close[bar]) / 3;
        Weighted // Price[bar] = (Low[bar] + High[bar] + 2 * Close[bar]) / 4;
    }
}