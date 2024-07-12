using System;
using Acceptor;
using OrderAccumulatorApp.Consumer;
using QuickFix;

namespace OrderAccumulatorApp
{
    class Program
    {
        private const string HttpServerPrefix = "http://127.0.0.1:5080/";

        [STAThread]
        static void Main(string[] args)
        {
            string file = "OrderAccumulator.cfg";

            try
            {
                SessionSettings settings = new SessionSettings(file);
                IApplication executorApp = new OrderAccumulatorService();
                IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
                ILogFactory logFactory = new FileLogFactory(settings);
                ThreadedSocketAcceptor acceptor = new ThreadedSocketAcceptor(executorApp, storeFactory, settings, logFactory);
                HttpServer srv = new HttpServer(HttpServerPrefix, settings);

                acceptor.Start();
                srv.Start();

                if (OrderAccumulatorConsumer.shouldRun())
                {
                    new Thread(OrderAccumulatorConsumer.run).Start();
                }
                Console.Read();

                srv.Stop();
                acceptor.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("==FATAL ERROR==");
                Console.WriteLine(e.ToString());
            }
        }

    }
}