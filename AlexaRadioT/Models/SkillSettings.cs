using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Models
{
    public class SkillSettings
    {
        public Uri WebApplicationUrl { get; set; }
        public Uri PodcastRssFeed { get; set; }
        public Uri LiveStreamUrl { get; set; }
        public Uri PodcastLastAPIFormatString { get; set; }
        public Uri PodcastDetailsAPIFormatString { get; set; }
        public string LiveStreamScheduledDay { get; set; }
        public int LiveStreamScheduledHourMsk { get; set; }
    }
}
