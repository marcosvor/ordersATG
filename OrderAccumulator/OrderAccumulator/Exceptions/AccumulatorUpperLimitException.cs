using System;

namespace OrderAccumulatorApp.Exceptions;

public class AccumulatorUpperLimitException : Exception
{
    public AccumulatorUpperLimitException()
    {
    }

    public AccumulatorUpperLimitException(string msg)
        : base("Upper limit reached: " + msg)
    {
    }

    public AccumulatorUpperLimitException(QuickFix.FIX44.NewOrderSingle n)
        : base($"Upper limit reached on Order: {n.ClOrdID}, for company: {n.Symbol}")
    {
    }

    public AccumulatorUpperLimitException(string msg, Exception inner)
        : base(msg, inner)
    {
    }
}