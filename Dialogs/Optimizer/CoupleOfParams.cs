namespace Forex_Strategy_Builder.Dialogs.Optimizer
{
    /// <summary>
    /// The numbers of the parameters into the couple
    /// </summary>
    public struct CoupleOfParams
    {
        int  _param1;
        int  _param2;
        bool _isPassed;

        public int Param1 { get { return _param1; } set { _param1 = value; } }
        public int Param2 { get { return _param2; } set { _param2 = value; } }
        public bool IsPassed { get { return _isPassed; } set { _isPassed = value; } }
    }
}