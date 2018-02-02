using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

namespace AlexaRadioT.Intents
{
    public class PlayLastPodcastIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            PodcastEnity podcast = RadioT.GetLatestPodcasts(1).First();
            return PlayPodcastNumberIntent.PlayPodcastNumber(request, podcast.Number);
        }
    }
}