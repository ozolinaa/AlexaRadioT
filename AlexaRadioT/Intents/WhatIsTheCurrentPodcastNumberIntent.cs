using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaRadioT.Intents
{
    public class WhatIsTheCurrentPodcastNumberIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            int lastPodcastNumber = RadioT.LastPodcastNumber();

            AlexaResponse response = new AlexaResponse();
            response.Response.OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
            {
                Type = "SSML",
                Ssml = string.Format("<speak>{0} {1}</speak>",
                    "The current Radio-T podcast number is",
                    lastPodcastNumber + 1)
            };
            response.Response.Card = new AlexaResponse.ResponseAttributes.CardAttributes()
            {
                Type = "Simple",
                Title = "Current Radio-T Number",
                Content = (lastPodcastNumber + 1).ToString()
            };

            return response;
        }
    }
}