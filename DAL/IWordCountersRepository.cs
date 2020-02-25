using Microsoft.EntityFrameworkCore;
using System;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    public interface IWordCountersRepository
    {
        DbContext DbContext { get; }

        int Create(CountResultRow countResultRow);

        CountResultRow GetResultByCorrelationId(Guid id);
    }
}