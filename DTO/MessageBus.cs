using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class MessageBus
    {
        public void PushMessageToQueue<T>(string queueName, T messageDto)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            string jsonBody = messageDto.ToString();
            var body = Encoding.UTF8.GetBytes(jsonBody);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: properties,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", messageDto);

        }
        public void PullMessageFromQueue<T>(string queueName, Action<T> onReceive)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            Console.WriteLine(" [*] Waiting for messages.");
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var deserializeBody = JsonConvert.DeserializeObject<T>(message);
                Console.WriteLine(" [x] Received {0}", deserializeBody);
                onReceive(deserializeBody);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
