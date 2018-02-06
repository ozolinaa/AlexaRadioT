using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Models
{
    public class SkillSettings
    {
        public Uri WebApplicationUrl { get; set; }
        public bool ProxyLiveStreamAudio { get; set; }
        public bool ProxyPodcastAudio { get; set; }
        public string[] AllowedDomainsForPodcastAudioProxy { get; set; }
        public string[] AllowedDomainsForPodcastLiveStreamProxy { get; set; }
        public Uri PodcastRssFeed { get; set; }
        public Uri PodcastLiveStreamUrl { get; set; }
        public Uri PodcastLastAPIFormatString { get; set; }
        public Uri PodcastDetailsAPIFormatString { get; set; }
        public string LiveStreamScheduledDay { get; set; }
        public int LiveStreamScheduledHourMsk { get; set; }
    }
}
