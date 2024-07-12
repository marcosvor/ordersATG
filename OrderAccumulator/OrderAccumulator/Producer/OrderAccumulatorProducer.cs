using System;
using System.Text.Json;
using Confluent.Kafka;
using OrderAccumulatorApp.Models;

namespace OrderAccumulatorApp.Producer
{
    class OrderAccumulatorProducer
    {

        ProducerConfig conf = new ProducerConfig { BootstrapServers = "localhost:9092" }; 

        public void Produce(Orders order)
        {
            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                string orderString = JsonSerializer.Serialize(order);
                p.Produce("orders-accumulator", new Message<Null, string> { Value = orderString }, handler);
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
