namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Band Indicators Logic
    /// </summary>
    public enum BandIndLogic
    {
        The_bar_opens_below_the_Upper_Band,
        The_bar_opens_above_the_Upper_Band,
        The_bar_opens_below_the_Lower_Band,
        The_bar_opens_above_the_Lower_Band,
        The_position_opens_below_the_Upper_Band,
        The_position_opens_above_the_Upper_Band,
        The_position_opens_below_the_Lower_Band,
        The_position_opens_above_the_Lower_Band,
        The_bar_opens_below_the_Upper_Band_after_opening_above_it,
        The_bar_opens_above_the_Upper_Band_after_opening_below_it,
        The_bar_opens_below_the_Lower_Band_after_opening_above_it,
        The_bar_opens_above_the_Lower_Band_after_opening_below_it,
        The_bar_closes_below_the_Upper_Band,
        The_bar_closes_above_the_Upper_Band,
        The_bar_closes_below_the_Lower_Band,
        The_bar_closes_above_the_Lower_Band,
        It_does_not_act_as_a_filter
    }
}