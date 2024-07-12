using System;
using QuickFix;
using QuickFix.Fields;
using System.Collections.Generic;
using System.Timers;

namespace OrderGeneratorApp
{
    public class OrderGeneratorService : QuickFix.MessageCracker, QuickFix.IApplication
    {
        Session? _session;

        #region IApplication interface overrides
        public void OnCreate(SessionID sessionID)
        {
            _session = Session.LookupSession(sessionID);
        }

        public void OnLogon(SessionID sessionID) { Console.WriteLine("Logon - " + sessionID.ToString()); }
        public void OnLogout(SessionID sessionID) { Console.WriteLine("Logout - " + sessionID.ToString()); }

        public void FromAdmin(Message message, SessionID sessionID) { }
        public void ToAdmin(Message message, SessionID sessionID) { }

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("IN:  " + message.ToString());
            try
            {
                Crack(message, sessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==Cracker exception==");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void ToApp(Message message, SessionID sessionID)
        {
            Console.WriteLine();
            Console.WriteLine("OUT: " + message.ToString());
        }
        #endregion


        #region MessageCracker handlers
        public void OnMessage(QuickFix.FIX44.ExecutionReport m, SessionID s)
        {
            Console.WriteLine("Received execution report");
            Console.WriteLine("OUT: " + m.ToString());
            Console.WriteLine($"Symbol: {m.Symbol.getValue()}, ExecType: {(m.ExecType.getValue() == 0 ? "NEW" : "REJECTED")}, Side: {(m.Side.getValue() == 1 ? "BUY" : "SELL")}, Qty: {m.OrderQty.getValue()}, Price: {m.Price.getValue()}, OrderTotal: {m.OrderQty.getValue() * m.Price.getValue()}");
        }

        public void SendMessage(Message m)
        {
            if (_session != null)
                _session.Send(m);
            else
            {
                Console.WriteLine("Can't send message: session not created.");
            }
        }
        #endregion

        public void Run(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Order event raised at {0}", e.SignalTime);
            try
            {
                QuickFix.FIX44.NewOrderSingle m = CreateMessage();

                if (m != null)
                {
                    m.Header.GetString(Tags.BeginString);

                    SendMessage(m);
                }
            }
            catch (System.Exception err)
            {
                Console.WriteLine("Message Not Sent: " + err.Message);
                Console.WriteLine("StackTrace: " + err.StackTrace);
            }
        }

        public QuickFix.FIX44.NewOrderSingle CreateMessage()
        {
            QuickFix.FIX44.NewOrderSingle newOrder = new QuickFix.FIX44.NewOrderSingle(
                QueryUuid(),
                QuerySymbol(),
                QuerySide(),
                new TransactTime(DateTime.Now),
                new OrdType(OrdType.LIMIT));

            newOrder.Set(new HandlInst('1'));
            newOrder.Set(QueryOrderQty());
            newOrder.Set(QueryTimeInForce());
            newOrder.Set(QueryPrice());

            return newOrder;
        }

        public ClOrdID QueryUuid()
        {
            Guid _uuidGenerator = Guid.NewGuid();
            return new ClOrdID(_uuidGenerator.ToString());
        }

        public Symbol QuerySymbol()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int r = rand.Next(0, 2);
            string s = "";
            switch (r)
            {
                case 0: s = "PETR4"; break;
                case 1: s = "VALE3"; break;
                case 2: s = "VIIA4"; break;
            }
            return new Symbol(s);
        }

        public Side QuerySide()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int r = rand.Next(0, 1);
            if (r % 2 == 0) { return new Side(Side.BUY); }
            else { return new Side(Side.SELL); }
        }

        public OrderQty QueryOrderQty()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int r = rand.Next(1, 100000);
            return new OrderQty(r);
        }

        public TimeInForce QueryTimeInForce()
        {
            return new TimeInForce(TimeInForce.IMMEDIATE_OR_CANCEL);
        }

        public Price QueryPrice()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            decimal r = (decimal)(rand.Next(1, 100000) * 0.01);
            return new Price(r);
        }
    }
}