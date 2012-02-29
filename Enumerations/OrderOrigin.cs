namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Order origin
    /// </summary>
    public enum OrderOrigin
    {
        None,
        Strategy,
        PermanentStopLoss,
        PermanentTakeProfit,
        BreakEven,
        BreakEvenActivation,
        MarginCall
    }
}