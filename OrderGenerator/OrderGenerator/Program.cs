using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using QuickFix;

namespace OrderGeneratorApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string file = "OrderGenerator.cfg";
            try
            {
                QuickFix.SessionSettings settings = new QuickFix.SessionSettings(file);
                OrderGeneratorService application = new OrderGeneratorService();
                IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
                ILogFactory logFactory = new FileLogFactory(settings);
                QuickFix.Transport.SocketInitiator initiator = new QuickFix.Transport.SocketInitiator(application, storeFactory, settings, logFactory);
                System.Timers.Timer orderInterval = new System.Timers.Timer();
                orderInterval.Interval = 1000; // 1 segundo
                orderInterval.Elapsed += application.Run;

                initiator.Start();
                orderInterval.Enabled = true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}