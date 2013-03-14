// Position class
// Part of Forex Strategy Builder
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;

namespace Forex_Strategy_Builder
{
    /// <summary>
    /// Class Position
    /// </summary>
    public class Position
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Position()
        {
            PosDir = PosDirection.None;
            Transaction = Transaction.None;
        }

        /// <summary>
        /// The amount of the position
        /// </summary>
        public Transaction Transaction { get; set; }

        /// <summary>
        /// The direction of the position
        /// </summary>
        public PosDirection PosDir { get; set; }

        /// <summary>
        /// The amount of the position
        /// </summary>
        public int PosNumb { get; set; }

        /// <summary>
        /// The bar when the position was open
        /// </summary>
        public int OpeningBar { get; set; }

        /// <summary>
        /// The amount of the position in lots
        /// </summary>
        public double PosLots { get; set; }

        /// <summary>
        /// The corrected position's price
        /// </summary>
        public double PosPrice { get; set; }

        /// <summary>
        /// Absolute mode Permanent SL
        /// </summary>
        public double AbsoluteSL { get; set; }

        /// <summary>
        /// Absolute mode Permanent TP
        /// </summary>
        public double AbsoluteTP { get; set; }

        /// <summary>
        /// Break Even
        /// </summary>
        public bool IsBreakEvenActivated { get; set; }

        /// <summary>
        /// The required margin
        /// </summary>
        public double RequiredMargin { get; set; }

        /// <summary>
        /// Gets the free margin
        /// </summary>
        public double FreeMargin { get { return MoneyEquity - RequiredMargin; } }

        /// <summary>
        /// The forming order number
        /// </summary>
        public int FormOrdNumb { get; set; }

        /// <summary>
        /// The forming order price
        /// </summary>
        public double FormOrdPrice { get; set; }

        /// <summary>
        /// The position Profit Loss
        /// </summary>
        public double ProfitLoss { get; set; }

        /// <summary>
        /// The position Floating Profit Loss
        /// </summary>
        public double FloatingPL { get; set; }

        /// <summary>
        /// Account balance at the end of the session
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// Account equity at the end of the session
        /// </summary>
        public double Equity { get; set; }

        /// <summary>
        /// Charged spread [pips]
        /// </summary>
        public double Spread { get; set; }

        /// <summary>
        /// Charged rollover
        /// </summary>
        public double Rollover { get; set; }

        /// <summary>
        /// Charged commission [pips]
        /// </summary>
        public double Commission { get; set; }

        /// <summary>
        /// Charged slippage [pips]
        /// </summary>
        public double Slippage { get; set; }

        /// <summary>
        /// The position Profit Loss in currency
        /// </summary>
        public double MoneyProfitLoss { get; set; }

        /// <summary>
        /// The position Floating Profit Loss in currency
        /// </summary>
        public double MoneyFloatingPL { get; set; }

        /// <summary>
        /// Account balance at the end of the session in currency
        /// </summary>
        public double MoneyBalance { get; set; }

        /// <summary>
        /// Account equity at the end of the session in currency
        /// </summary>
        public double MoneyEquity { get; set; }

        /// <summary>
        /// Charged spread in currency
        /// </summary>
        public double MoneySpread { get; set; }

        /// <summary>
        /// Charged rollover in currency
        /// </summary>
        public double MoneyRollover { get; set; }

        /// <summary>
        /// Charged commission in currency
        /// </summary>
        public double MoneyCommission { get; set; }

        /// <summary>
        /// Charged slippage in currency
        /// </summary>
        public double MoneySlippage { get; set; }

        /// <summary>
        /// Gets the position's icon
        /// </summary>
        public PositionIcons PositionIcon
        {
            get
            {
                var icon = PositionIcons.Square;

                switch (Transaction)
                {
                    case Transaction.Open:
                        icon = PosDir == PosDirection.Long ? PositionIcons.Buy : PositionIcons.Sell;
                        break;
                    case Transaction.Close:
                        icon = PositionIcons.Close;
                        break;
                    case Transaction.Transfer:
                        icon = PosDir == PosDirection.Long ? PositionIcons.TransferLong : PositionIcons.TransferShort;
                        break;
                    case Transaction.Add:
                        icon = PosDir == PosDirection.Long ? PositionIcons.AddLong : PositionIcons.AddShort;
                        break;
                    case Transaction.Reverse:
                    case Transaction.Reduce:
                        icon = PosDir == PosDirection.Long ? PositionIcons.ReverseLong : PositionIcons.ReverseShort;
                        break;
                }

                return icon;
            }
        }

        /// <summary>
        /// Gets the position's icon
        /// </summary>
        public static Image PositionIconImage(PositionIcons icon)
        {
                Image img = Properties.Resources.pos_square;

                switch (icon)
                {
                    case PositionIcons.Buy:
                        img = Properties.Resources.pos_buy;
                        break;
                    case PositionIcons.Sell:
                        img = Properties.Resources.pos_sell;
                        break;
                    case PositionIcons.Close:
                        img = Properties.Resources.pos_close;
                        break;
                    case PositionIcons.TransferLong:
                        img = Properties.Resources.pos_transfer_long;
                        break;
                    case PositionIcons.TransferShort:
                        img = Properties.Resources.pos_transfer_short;
                        break;
                    case PositionIcons.AddLong:
                        img = Properties.Resources.pos_add_long;
                        break;
                    case PositionIcons.AddShort:
                        img = Properties.Resources.pos_add_short;
                        break;
                    case PositionIcons.ReverseLong:
                        img = Properties.Resources.pos_revers_long;
                        break;
                    case PositionIcons.ReverseShort:
                        img = Properties.Resources.pos_revers_short;
                        break;
                    case PositionIcons.Square:
                        img = Properties.Resources.pos_square;
                        break;
                }

            return img;
        }

        /// <summary>
        /// Deep copy of the position
        /// </summary>
        public Position Copy()
        {
            var position = new Position
            {
                Transaction = Transaction,
                PosDir = PosDir,
                OpeningBar = OpeningBar,
                FormOrdNumb = FormOrdNumb,
                FormOrdPrice = FormOrdPrice,
                PosNumb = PosNumb,
                PosLots = PosLots,
                PosPrice = PosPrice,
                AbsoluteSL = AbsoluteSL,
                AbsoluteTP = AbsoluteTP,
                IsBreakEvenActivated = IsBreakEvenActivated,
                RequiredMargin = RequiredMargin,
                ProfitLoss = ProfitLoss,
                FloatingPL = FloatingPL,
                Balance = Balance,
                Equity = Equity,
                Spread = Spread,
                Rollover = Rollover,
                Commission = Commission,
                Slippage = Slippage,
                MoneyProfitLoss = MoneyProfitLoss,
                MoneyFloatingPL = MoneyFloatingPL,
                MoneyBalance = MoneyBalance,
                MoneyEquity = MoneyEquity,
                MoneySpread = MoneySpread,
                MoneyRollover = MoneyRollover,
                MoneyCommission = MoneyCommission,
                MoneySlippage = MoneySlippage
            };

            return position;
        }

        /// <summary>
        /// Represents the position.
        /// </summary>
        public override string ToString()
        {
            string pos = "";
            string nl = Environment.NewLine;
            string ac = Configs.AccountCurrency;

            pos += "Pos Numb             "      + (PosNumb + 1)      + nl;
            pos += "Transaction          "      + Transaction        + nl;
            pos += "Direction            "      + PosDir             + nl;
            pos += "Opening Bar          "      + (OpeningBar + 1)   + nl;
            pos += "Order Number         "      + (FormOrdNumb + 1)  + nl;
            pos += "Order Price          "      + FormOrdPrice       + nl;
            pos += "Position Lots        "      + PosLots            + nl;
            pos += "Position Price       "      + PosPrice           + nl;
            pos += "Req. Margin [" + ac + "]  " + RequiredMargin     + nl;
            pos += "---------------------------------"               + nl;
            pos += "Abs Permanent SL     "      + AbsoluteSL         + nl;
            pos += "Abs Permanent TP     "      + AbsoluteTP         + nl;
            pos += "Break Even Activated "      + IsBreakEvenActivated + nl;
            pos += "---------------------------------"               + nl;
            pos += "Spread      [pips] "        + Spread             + nl;
            pos += "Rollover    [pips] "        + Rollover           + nl;
            pos += "Commission  [pips] "        + Commission         + nl;
            pos += "Slippage    [pips] "        + Slippage           + nl;
            pos += "Floating PL [pips] "        + FloatingPL         + nl;
            pos += "Profit Loss [pips] "        + ProfitLoss         + nl;
            pos += "Balance     [pips] "        + Balance            + nl;
            pos += "Equity      [pips] "        + Equity             + nl;
            pos += "---------------------------------"               + nl;
            pos += "Spread      [" + ac + "]  " + MoneySpread        + nl;
            pos += "Rollover    [" + ac + "]  " + MoneyRollover      + nl;
            pos += "Commission  [" + ac + "]  " + MoneyCommission    + nl;
            pos += "Slippage    [" + ac + "]  " + MoneySlippage      + nl;
            pos += "Floating PL [" + ac + "]  " + MoneyFloatingPL    + nl;
            pos += "Profit Loss [" + ac + "]  " + MoneyProfitLoss    + nl;
            pos += "Balance     [" + ac + "]  " + MoneyBalance       + nl;
            pos += "Equity      [" + ac + "]  " + MoneyEquity        + nl;

            return pos;
        }
    }
}
