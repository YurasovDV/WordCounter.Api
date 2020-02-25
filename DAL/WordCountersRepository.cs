using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    public class WordCountersRepository : IWordCountersRepository
    {
        private readonly ILogger _logger;
        private readonly CountResultsContext _countResultsContext;

        public WordCountersRepository(ILogger<WordCountersRepository> logger, CountResultsContext countResultsContext)
        {
            _logger = logger;
            _countResultsContext = countResultsContext;
        }

        public int Create(CountResultRow countResultRow)
        {
            _countResultsContext.CountResults.Add(countResultRow);
            _countResultsContext.SaveChanges();
            return countResultRow.Id;
        }

        public DbContext DbContext => _countResultsContext;

        public CountResultRow GetResultByCorrelationId(Guid id)
        {
            // when will it end
            return Task.Run(() => _countResultsContext.CountResults.FirstOrDefaultAsync(r => r.CorrelationId == id)).Result;
        }
    }
}
