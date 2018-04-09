using AlexaRadioT.Intents;
using AlexaRadioT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Events
{
    public class LaunchRequest : IAlexaEvent
    {
        public void ProcessRequest(AlexaRequest request)
        {
            AlexaResponse response = new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>Welcome to Radio-T! Would you like to listen the last podcast?</speak>"
                    },
                    ShouldEndSession = true,
                },
                Session = new AlexaRequest.SessionCustomAttributes()
                {
                    NextIntentName = typeof(PlayLastPodcastIntent).ToString()
                }
            };
        }
    }
}
