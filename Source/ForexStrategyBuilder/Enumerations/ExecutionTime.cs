namespace Forex_Strategy_Builder
{
    /// <summary>
    /// The type of the time execution indicator.
    /// It is used with the indicators, which set opening / closing position price.
    /// </summary>
    public enum ExecutionTime
    {
        DuringTheBar,   // The opening / closing price can be everywhere in the bar.
        AtBarOpening,   // The opening / closing price is at the beginning of the bar.
        AtBarClosing,   // The opening / closing price is at the end of the bar.
        CloseAndReverse // For the close and reverse logic.
    }
}