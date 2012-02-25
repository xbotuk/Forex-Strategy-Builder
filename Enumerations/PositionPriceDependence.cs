namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Show dependence from the position's opening price
    /// </summary>
    public enum PositionPriceDependence
    {
        None,
        PriceBuyHigher,
        PriceBuyLower,
        PriceSellHigher,
        PriceSellLower,
        BuyHigherSellLower,
        BuyLowerSelHigher,
        PriceBuyCrossesUpBandInwards,
        PriceBuyCrossesUpBandOutwards,
        PriceBuyCrossesDownBandInwards,
        PriceBuyCrossesDownBandOutwards,
        PriceSellCrossesUpBandInwards,
        PriceSellCrossesUpBandOutwards,
        PriceSellCrossesDownBandInwards,
        PriceSellCrossesDownBandOutwards
    }
}