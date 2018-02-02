using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

namespace AlexaRadioT.Intents
{
    public class WhatIsTheLastPodcastNumberIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            if (request.Session.Attributes == null)
                request.Session.Attributes = new AlexaRequest.SessionCustomAttributes();

            int lastPodcastNumber = RadioT.LastPodcastNumber();

            AlexaResponse response = new AlexaResponse() {
                Response = new AlexaResponse.ResponseAttributes() {
                     OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes() {
                         Type = "SSML",
                         Ssml = string.Format("<speak>{0} {1}</speak>",
                            "The last Radio-T podcast had number",
                            lastPodcastNumber)
                     },
                      Card = new AlexaResponse.ResponseAttributes.CardAttributes() {
                          Type = "Simple",
                          Title = "Last Radio-T Number",
                          Content = lastPodcastNumber.ToString()
                      }
                 }
            };

            return response;
        }
    }
}
