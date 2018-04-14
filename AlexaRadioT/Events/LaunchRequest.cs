using AlexaRadioT.Intents;
using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Events
{
    public class LaunchRequest : IAlexaEvent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            PodcastEnity podcast = RadioT.GetLatestPodcasts(1).First();
            return PlayPodcastNumberIntent.PlayPodcast(request, podcast.Number, "<speak>Welcome to Radio-T! Playing latest podcast</speak>");

            //AlexaResponse response = new AlexaResponse()
            //{
            //    Response = new AlexaResponse.ResponseAttributes()
            //    {
            //        OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
            //        {
            //            Type = "SSML",
            //            Ssml = "<speak>Welcome to Radio-T! Playing latest podcast</speak>"
            //        },
            //        ShouldEndSession = true,
            //    },
            //    Session = new AlexaRequest.SessionCustomAttributes()
            //    {
            //        NextIntentName = typeof(PlayLastPodcastIntent).ToString()
            //    }
            //};

            //return response;
        }
    }
}
