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
        public static Dictionary<int, PodcastEnity> podcastDetailsCache = new Dictionary<int, PodcastEnity>();

        private static DateTime GetTimeInMoscow()
        {
            return DateTime.UtcNow.AddHours(3);
        }

        public static Uri GetUriForPodcast(int podcastNumber)
        {
            return new Uri(ApplicationSettingsService.Skill.WebApplicationUrl, "PlayList/Podcast/" + podcastNumber);
        }

        public static Uri GetUriForLiveStream()
        {
            return new Uri(ApplicationSettingsService.Skill.WebApplicationUrl, "PlayList/LiveStream");
        }



        public static DateTime WhenNextLiveStream()
        {
            DayOfWeek liveStreamScheduledDay = Enum.Parse<DayOfWeek>(ApplicationSettingsService.Skill.LiveStreamScheduledDay, true);
            int liveStreamScheduledHourMsk = ApplicationSettingsService.Skill.LiveStreamScheduledHourMsk;

            DateTime tm = GetTimeInMoscow();
            DateTime timeNextStream = new DateTime(tm.Year, tm.Month, tm.Day, liveStreamScheduledHourMsk, 0, 0);
            int num_days = liveStreamScheduledDay - timeNextStream.DayOfWeek;
            if (num_days < 0) num_days += 7;
            timeNextStream = timeNextStream.AddDays(num_days);

            //If same day, but already passed push to next week
            if ((timeNextStream - tm).TotalMinutes < 0)
                timeNextStream = timeNextStream.AddDays(7);

            return timeNextStream;
        }

        public static TimeSpan HowLongForNextLiveStream()
        {
            DateTime timeInMoscow = GetTimeInMoscow();
            DateTime nextLiveStream = WhenNextLiveStream();
            TimeSpan realResult = nextLiveStream - timeInMoscow;

            double thresholdHours = 1.5;

            if (realResult.TotalHours < 24 * 7 - thresholdHours)
                return realResult;

            //This will return negative TimeSpan that indicates for how long it is Live
            return nextLiveStream.AddDays(-7) - timeInMoscow;
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
            if (podcastDetailsCache.TryGetValue(number, out PodcastEnity cachedPodcast))
                return cachedPodcast;

            string podcastsDetailsURLFormat = ApplicationSettingsService.Skill.PodcastDetailsAPIFormatString.ToString();

            PodcastEnity podcast = null;
            using (HttpClient client = new HttpClient())
            {
                string json = client.GetStringAsync(string.Format(podcastsDetailsURLFormat, number)).Result;
                podcast = JsonConvert.DeserializeObject<PodcastEnity>(json);
            }

            podcastDetailsCache.Add(number, podcast);
            return podcast;
        }

        public static int LastPodcastNumber()
        {
            return GetLatestPodcasts(1).First().Number;
        }
    }
}
