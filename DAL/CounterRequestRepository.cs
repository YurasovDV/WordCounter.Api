using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    public class CounterRequestRepository : ICounterRequestRepository
    {
        private readonly ILogger _logger;
        private readonly CountResultsContext _countResultsContext;

        public CounterRequestRepository(ILogger<CounterRequestRepository> logger, CountResultsContext countResultsContext)
        {
            _logger = logger;
            _countResultsContext = countResultsContext;
        }

        public int Create(CountRequest countRequestRow)
        {
            _countResultsContext.CountRequests.Add(countRequestRow);
            _countResultsContext.SaveChanges();
            return countRequestRow.Id;
        }

        public DbContext DbContext => _countResultsContext;
    }
}
