using QuickFix;
using QuickFix.Fields;

namespace OrderAccumulatorApp
{
    public static class Accumulator
    {
        public static readonly decimal ORDER_ACCUMULATOR_LOWER_LIMIT = 0;
        public static readonly decimal ORDER_ACCUMULATOR_UPPER_LIMIT = 100000000;
        public static List<QuickFix.FIX44.NewOrderSingle> orders = new List<QuickFix.FIX44.NewOrderSingle>();
        public static Dictionary<string, decimal> acc = new Dictionary<string, decimal>() {
            { "PETR4", 0.0m },
            { "VALE3", 0.0m },
            { "VIIA4", 0.0m }
        };
    }
}