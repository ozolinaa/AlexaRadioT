using AlexaRadioT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Store
{
    public static class Cache
    {
        private struct PodcastCache
        {
            public DateTime cachedDateTime;
            public PodcastEnity podcast;
        }

        private const int _cacheForMinutes = 5;

        private static Dictionary<int, PodcastCache> _podcastDetailsCache = new Dictionary<int, PodcastCache>();

        public static PodcastEnity PodcastGet(int number)
        {
            if (_podcastDetailsCache.TryGetValue(number, out PodcastCache cachedPodcast))
            {
                if ((DateTime.Now - cachedPodcast.cachedDateTime).TotalMinutes > _cacheForMinutes)
                    _podcastDetailsCache.Remove(number);
                else
                    return cachedPodcast.podcast;
            }
            return null;
        }

        public static void PodcastPut(PodcastEnity podcast)
        {
            if (_podcastDetailsCache.ContainsKey(podcast.Number))
                _podcastDetailsCache[podcast.Number] = new PodcastCache() { cachedDateTime = DateTime.Now, podcast = podcast };
            else
                _podcastDetailsCache.Add(podcast.Number, new PodcastCache() { cachedDateTime = DateTime.Now, podcast = podcast });
        }
    }
}
