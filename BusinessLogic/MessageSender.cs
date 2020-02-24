using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    class MessageSender : IMessageSender, IDisposable
    {
        private volatile IConnection queueConnection;
        private bool _disposed;
        private readonly IEnvironmentFacade _environment;
        private readonly Connector _connector;

        public static object _lock = new object();

        private readonly ILogger<MessageSender> _logger;

        public MessageSender(ILogger<MessageSender> logger, IEnvironmentFacade environment, Connector connector)
        {
            _logger = logger;
            _environment = environment;
            _connector = connector;
        }

        public void Send(BusinessMessage businessMessage)
        {
            if (queueConnection == null)
            {
                lock (_lock)
                {
                    if (queueConnection == null)
                    {
                        queueConnection = _connector.ConnectToQueue(_logger, _environment.BuildQueueSettings());
                    }
                }
            }

            try
            {
                _logger.LogDebug($"MessageSender:Send: '{businessMessage.CorrelationId}'");
                using (var channel = queueConnection.CreateModel())
                {
                    channel.ExchangeDeclare(Constants.ArticlesExchange, ExchangeType.Fanout);
                    string message = JsonConvert.SerializeObject(businessMessage);
                    channel.BasicPublish(Constants.ArticlesExchange, Constants.RoutingKey, basicProperties:null, body:Encoding.UTF8.GetBytes(message));
                    _logger.LogDebug($"MessageSender:Send: '{businessMessage.CorrelationId}': SUCCESS");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (queueConnection != null)
                {
                    queueConnection.Dispose();
                }
                _disposed = true;
                _logger.LogInformation("MessageSender disposed");
            }
            else
            {
                _logger.LogWarning("MessageSender: second 'Dispose' call");
            }
        }
    }
}
