using Newtonsoft.Json;

namespace WordCounterEndpoint
{
    public class Dto
    {
        [JsonProperty(PropertyName = "article")]
        public string Article { get; set; }
    }
}
