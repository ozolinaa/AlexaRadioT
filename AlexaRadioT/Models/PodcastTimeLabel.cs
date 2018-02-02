using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Models
{
    public class PodcastTimeLabel
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }

        public long GetOffset(DateTime PodcastStartTime)
        {
            return Convert.ToInt64((Time - PodcastStartTime).TotalMilliseconds);
        }
    }
}
