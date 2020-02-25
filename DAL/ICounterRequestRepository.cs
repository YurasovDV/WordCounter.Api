using Microsoft.EntityFrameworkCore;
using WordCounter.Common;

namespace WordCounterEndpoint
{
    public interface ICounterRequestRepository
    {
        DbContext DbContext { get; }

        int Create(CountRequest countRequestRow);
    }
}
