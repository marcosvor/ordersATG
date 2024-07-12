using System;

namespace OrderAccumulatorApp.Exceptions;

public class AccumulatorLowerLimitException : Exception
{
    public AccumulatorLowerLimitException()
    {
    }

    public AccumulatorLowerLimitException(string msg)
        : base("Lower limit reached: " + msg)
    {
    }

    public AccumulatorLowerLimitException(QuickFix.FIX44.NewOrderSingle n)
        : base($"Lower limit reached on Order: {n.ClOrdID}, for company: {n.Symbol}")
    {
    }

    public AccumulatorLowerLimitException(string msg, Exception inner)
        : base(msg, inner)
    {
    }

}