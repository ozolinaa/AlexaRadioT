using AlexaRadioT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace AlexaRadioT.Store
{
    public static class RadioT
    {
        private static DateTime GetTimeInMoscow()
        {
            return DateTime.UtcNow.AddHours(3);
        }

        public static Uri GetUriForPodcast(int episodeNumber)
        {
            Uri podcastUri = new Uri(string.Format(ApplicationSettingsService.Skill.PodcastAudioUrlFormatString.ToString(), episodeNumber));

            if (ApplicationSettingsService.Skill.ProxyPodcastAudio) {
                podcastUri = new Uri(ApplicationSettingsService.Skill.WebApplicationUrl, 
                    "proxyAudio?url=" + HttpUtility.UrlEncode(podcastUri.ToString()));
            }

            return podcastUri;
        }

        public static Uri GetUriForLiveStream()
        {
            Uri streamUrl = ApplicationSettingsService.Skill.PodcastLiveStreamUrl;

            if (ApplicationSettingsService.Skill.ProxyLiveStreamAudio)
            {
                streamUrl = new Uri(ApplicationSettingsService.Skill.WebApplicationUrl, 
                    "proxyAudio?url=" + HttpUtility.UrlEncode(streamUrl.ToString()));
            }

            return streamUrl;
        }

        public static DateTime WhenNextLiveStream()
        {
            DayOfWeek liveStreamScheduledDay = Enum.Parse<DayOfWeek>(ApplicationSettingsService.Skill.LiveStreamScheduledDay, true);
            int liveStreamScheduledHourMsk = ApplicationSettingsService.Skill.LiveStreamScheduledHourMsk;
            //TODO Need to fix this
            DateTime timeInMosow = GetTimeInMoscow();
            int num_days = liveStreamScheduledDay - timeInMosow.DayOfWeek;
            if (num_days < 0) num_days += 7;
            DateTime nextStream = timeInMosow.AddDays(num_days);
            int num_hours = liveStreamScheduledHourMsk - nextStream.Hour;
            if (num_hours < 0) num_hours += 24;
            nextStream = nextStream.AddHours(num_hours);
            nextStream = nextStream.AddMinutes(-1 * nextStream.Minute);
            nextStream = nextStream.AddSeconds(-1 * nextStream.Second);

            return nextStream;
        }

        public static TimeSpan HowLongForNextLiveStream()
        {
            TimeSpan realResult = WhenNextLiveStream() - GetTimeInMoscow();


            //if (realResult.TotalHours < 24 * 7 - thresholdHours)
            //    return realResult;

            //This will return negative TimeSpan that indicates for how long it is Live
            return WhenNextLiveStream().AddDays(-7) - GetTimeInMoscow();
        }

        public static IOrderedEnumerable<PodcastRssItem> GetLatestPodcastsFromRSS()
        {
            List<PodcastRssItem> results = new List<PodcastRssItem>();

            string rssFeed = ApplicationSettingsService.Skill.PodcastRssFeed.ToString();

            XmlDocument rssXmlDoc = new XmlDocument();
            rssXmlDoc.Load(rssFeed);

            foreach (XmlNode podcastItemNode in rssXmlDoc.SelectNodes("rss/channel/item"))
                results.Add(new PodcastRssItem(podcastItemNode));

            return results.OrderByDescending(x => x.EpisodeNumber);
        }

        public static IOrderedEnumerable<PodcastEnity> GetLatestPodcasts(int count = 10)
        {
            string latestPodcastsURLFormat = ApplicationSettingsService.Skill.PodcastLastAPIFormatString.ToString();

            List<PodcastEnity> podcasts = null;
            using (HttpClient client = new HttpClient())
            {
                string json = client.GetStringAsync(string.Format(latestPodcastsURLFormat, count)).Result;
                podcasts = JsonConvert.DeserializeObject<List<PodcastEnity>>(json);
            }
            return podcasts.OrderByDescending(x => x.Date);
        }

        public static PodcastEnity GetPodcastDetails(int number)
        {
            string podcastsDetailsURLFormat = ApplicationSettingsService.Skill.PodcastDetailsAPIFormatString.ToString();

            PodcastEnity podcast = null;
            using (HttpClient client = new HttpClient())
            {
                string json = client.GetStringAsync(string.Format(podcastsDetailsURLFormat, number)).Result;
                podcast = JsonConvert.DeserializeObject<PodcastEnity>(json);
            }
            return podcast;
        }

        public static int LastPodcastNumber()
        {
            return GetLatestPodcasts(1).First().Number;
        }
    }
}
