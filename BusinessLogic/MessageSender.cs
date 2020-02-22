using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    class MessageSender : IMessageSender, IDisposable
    {
        private Lazy<IConnection> queueConnection = new Lazy<IConnection>(TryConnect, isThreadSafe: true);
        private bool _disposed;

        public ILogger<MessageSender> Logger { get; }

        public MessageSender(ILogger<MessageSender> logger)
        {
            Logger = logger;
        }

        private static IConnection TryConnect()
        {
            var _factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable(Constants.RabbitMqHost),
                Port = int.Parse(Environment.GetEnvironmentVariable(Constants.RabbitMqPort)),
                UserName = Environment.GetEnvironmentVariable(Constants.RabbitMqUser),
                Password = Environment.GetEnvironmentVariable(Constants.RabbitMqPass),
            };
            int retriesLeft = 6;
            while (true)
            {
                retriesLeft--;
                try
                {
                    var connection = _factory.CreateConnection();
                    return connection;
                }
                catch (Exception)
                {
                    if (retriesLeft == 0)
                    {
                        throw;
                    }
                    Thread.Sleep((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
                }
            }
        }

        public void Send(BusinessMessage businessMessage)
        {
            try
            {
                Logger.LogDebug($"MessageSender:Send: '{businessMessage.CorrelationId}'");
                using (var channel = queueConnection.Value.CreateModel())
                {
                    channel.ExchangeDeclare(Constants.ArticlesExchange, ExchangeType.Fanout);
                    string message = JsonConvert.SerializeObject(businessMessage);
                    channel.BasicPublish(Constants.ArticlesExchange, Constants.RoutingKey, basicProperties:null, body:Encoding.UTF8.GetBytes(message));
                    Logger.LogDebug($"MessageSender:Send: '{businessMessage.CorrelationId}': SUCCESS");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (queueConnection.IsValueCreated)
                {
                    queueConnection.Value.Dispose();
                }
                _disposed = true;
                Logger.LogInformation("MessageSender disposed");
            }
            else
            {
                Logger.LogWarning("MessageSender: second 'Dispose' call");
            }
        }
    }
}
