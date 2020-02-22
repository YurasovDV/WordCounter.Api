using WordCounter.Common;

namespace WordCounterEndpoint
{
    public interface IMessageSender
    {
        void Send(BusinessMessage businessMessage);
    }
}