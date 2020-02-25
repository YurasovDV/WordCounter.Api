using Microsoft.AspNetCore.Mvc;
using System;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    public interface ICountersService
    {
        int CreateRequest(CountRequest row);

        int CreateResult(CountResultRow row);

        void WaitForDb();

        CountResultRow GetResultByCorrelationId(Guid id);
    }
}