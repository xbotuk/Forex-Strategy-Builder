namespace Forex_Strategy_Builder
{
    /// <summary>
    /// The type of indicator chart.
    /// Sets the chart type of an indicator component.
    /// </summary>
    public enum IndChartType
    {
        NoChart,   // This component is not drawn on the chart.
        Line,      // Line for the main or a separated chart: Alligator, MA, RSI, Momentum
        Dot,       // For the main chart: Parabolic SAR
        Histogram, // Histogram on a separated chart: MACD Histogram, Aroon
        Level,     // Fibonacci, Donchian Channel
        CloudUp,   // Ichimoku Kinko Hyo
        CloudDown  // Ichimoku Kinko Hyo
    }
}