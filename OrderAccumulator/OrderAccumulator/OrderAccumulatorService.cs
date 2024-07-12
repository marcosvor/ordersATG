using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuickFix;
using QuickFix.Fields;
using OrderAccumulatorApp.Exceptions;
using OrderAccumulatorApp.Producer;
using OrderAccumulatorApp.Models;
using OrderAccumulatorApp.Consumer;

namespace OrderAccumulatorApp
{
    public class OrderAccumulatorService : QuickFix.MessageCracker, QuickFix.IApplication
    {
        int orderID = 0;
        int execID = 0;

        private string GenOrderID() { return (++orderID).ToString(); }
        private string GenExecID() { return (++execID).ToString(); }

        #region QuickFix.Application Methods

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("IN:  " + message);
            Crack(message, sessionID);
        }

        public void ToApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("OUT: " + message);
        }

        public void FromAdmin(Message message, SessionID sessionID) { }
        public void OnCreate(SessionID sessionID) { }
        public void OnLogout(SessionID sessionID) { }
        public void OnLogon(SessionID sessionID) { }
        public void ToAdmin(Message message, SessionID sessionID) { }
        #endregion

        #region MessageCracker overloads

        public void OnMessage(QuickFix.FIX44.NewOrderSingle n, SessionID s)
        {
            decimal orderTotal = n.Price.getValue() * n.OrderQty.getValue();
            string symbol = n.Symbol.ToString();
            QuickFix.FIX44.ExecutionReport report;

            try
            {
                CheckLimits(n);
                Accumulator.acc[symbol] += orderTotal;
                Accumulator.orders.Add(n);
                report = CreateReport(n, new ExecType(ExecType.NEW));
                SendToProducer(n, "NEW", orderTotal);
                SendReport(report, s);
            }
            catch (AccumulatorLowerLimitException)
            {
                report = CreateReport(n, new ExecType(ExecType.REJECTED));
                SendToProducer(n, "REJECTED", orderTotal);
                SendReport(report, s);
            }
            catch (AccumulatorUpperLimitException)
            {
                report = CreateReport(n, new ExecType(ExecType.REJECTED));
                SendToProducer(n, "REJECTED", orderTotal);
                SendReport(report, s);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }

        #endregion

        private void SendReport(QuickFix.FIX44.ExecutionReport report, SessionID s)
        {
            try
            {
                Session.SendToTarget(report, s);
            }
            catch (SessionNotFound ex)
            {
                Console.WriteLine("==session not found exception!==");
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private QuickFix.FIX44.ExecutionReport CreateReport(QuickFix.FIX44.NewOrderSingle n, ExecType execType)
        {
            Symbol symbol = n.Symbol;
            Side side = n.Side;
            OrdType ordType = n.OrdType;
            OrderQty orderQty = n.OrderQty;
            Price price = n.Price;
            ClOrdID clOrdID = n.ClOrdID;

            QuickFix.FIX44.ExecutionReport report = new QuickFix.FIX44.ExecutionReport(
                new OrderID(GenOrderID()),
                new ExecID(GenExecID()),
                execType,
                new OrdStatus(OrdStatus.FILLED),
                symbol,
                side,
                new LeavesQty(0),
                new CumQty(orderQty.getValue()),
                new AvgPx(price.getValue()));

            report.Set(clOrdID);
            report.Set(symbol);
            report.Set(orderQty);
            report.Set(new LastQty(orderQty.getValue()));
            report.Set(new Price(price.getValue()));

            if (n.IsSetAccount())
                report.SetField(n.Account);

            return report;
        }

        private void SendToProducer(QuickFix.FIX44.NewOrderSingle n, string exType, decimal orderTotal)
        {
            if (!OrderAccumulatorConsumer.shouldRun()) return;
            Orders order = new Orders
            {
                Symbol = n.Symbol.getValue(),
                Side = n.Side.getValue() == 1 ? "BUY" : "SELL",
                OrderTotal = orderTotal,
                OrderQty = (int)n.OrderQty.getValue(),
                Price = n.Price.getValue(),
                ExecType = exType
            };
            OrderAccumulatorProducer prod = new OrderAccumulatorProducer();
            prod.Produce(order);
        }

        public void CheckLimits(QuickFix.FIX44.NewOrderSingle n)
        {
            decimal orderTotal = n.Price.getValue() * n.OrderQty.getValue();
            string symbol = n.Symbol.ToString();
            decimal lowerLimit = Accumulator.acc[symbol] - orderTotal;
            decimal upperLimit = Accumulator.acc[symbol] + orderTotal;
            bool isSelling = Side.SELL.ToString().Equals(n.Side.ToString());
            bool isBuying = Side.BUY.ToString().Equals(n.Side.ToString());

            if (isSelling && lowerLimit < Accumulator.ORDER_ACCUMULATOR_LOWER_LIMIT)
            {
                throw new AccumulatorLowerLimitException(n);
            }

            if (isBuying && upperLimit >= Accumulator.ORDER_ACCUMULATOR_UPPER_LIMIT)
            {
                throw new AccumulatorUpperLimitException(n);
            }
        }
    }
}