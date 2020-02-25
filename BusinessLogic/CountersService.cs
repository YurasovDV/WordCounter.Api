using Microsoft.Extensions.Logging;
using System;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    public class CountersService : ICountersService
    {
        private readonly ILogger<CountersService> _logger;
        private readonly Connector _connector;
        private readonly IEnvironmentFacade _environmentFacade;
        private readonly IWordCountersRepository _wordCountersRepository;
        private readonly ICounterRequestRepository _counterRequestRepository;

        public CountersService(ILogger<CountersService> logger, Connector connector, IEnvironmentFacade environmentFacade, IWordCountersRepository wordCountersRepository, ICounterRequestRepository counterRequestRepository)
        {
            _logger = logger;
            _connector = connector;
            _environmentFacade = environmentFacade;
            _wordCountersRepository = wordCountersRepository;
            _counterRequestRepository = counterRequestRepository;
        }

        public int CreateRequest(CountRequest row)
        {
            WaitForDb();
            return _counterRequestRepository.Create(row);
        }

        public int CreateResult(CountResultRow row)
        {
            WaitForDb();
            return _wordCountersRepository.Create(row);
        }

        public void WaitForDb()
        {
            Func<bool> ping = () =>
            {
                _counterRequestRepository.DbContext.Database.EnsureCreated();
                _logger.LogInformation("Connected to db");
                return true;
            };

            _connector.EnsureIsUp(_logger, _environmentFacade.BuildDbSettings(), ping);
        }

        public CountResultRow GetResultByCorrelationId(Guid id)
        {
            return _wordCountersRepository.GetResultByCorrelationId(id);
        }
    }
}
