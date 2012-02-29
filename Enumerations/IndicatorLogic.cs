namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Indicator Logic
    /// </summary>
    public enum IndicatorLogic
    {
        The_indicator_rises,
        The_indicator_falls,
        The_indicator_is_higher_than_the_level_line,
        The_indicator_is_lower_than_the_level_line,
        The_indicator_crosses_the_level_line_upward,
        The_indicator_crosses_the_level_line_downward,
        The_indicator_changes_its_direction_upward,
        The_indicator_changes_its_direction_downward,
        The_price_buy_is_higher_than_the_indicator_value,
        The_price_buy_is_lower_than_the_indicator_value,
        The_price_open_is_higher_than_the_indicator_value,
        The_price_open_is_lower_than_the_indicator_value,
        It_does_not_act_as_a_filter,
    }
}