using System.Text.Json;
using System.Threading;
using Confluent.Kafka;
using OrderAccumulatorApp.DB;
using OrderAccumulatorApp.Models;

namespace OrderAccumulatorApp.Consumer
{
    public static class OrderAccumulatorConsumer
    {
        static ConsumerConfig conf = new ConsumerConfig
        {
            GroupId = "orders-consumer-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        public static void run()
        {

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe("orders-accumulator");
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume();
                            using (var dbContext = new OrderAccumulatorDbContext())
                            {
                                Orders order = JsonSerializer.Deserialize<Orders>(cr.Message.Value);
                                if (order != null)
                                {
                                    dbContext.Add(order);
                                    dbContext.SaveChanges();
                                }
                            }

                            Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (Exception)
                {
                    c.Close();
                }
            }
        }

        public static bool shouldRun()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
            IConfiguration config = builder.Build();
            var dbConnectionConfig = config.GetSection("DbConnection").Get<OrderAccumulatorDbConfig>();
            return dbConnectionConfig != null &&
                    dbConnectionConfig.shouldRun &&
                    dbConnectionConfig.shouldRun == true;
        }
    }
}
