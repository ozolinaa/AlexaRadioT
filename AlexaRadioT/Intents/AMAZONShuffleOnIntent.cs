using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;

namespace AlexaRadioT.Intents
{
    public class AMAZONShuffleOnIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            AlexaResponse response = new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    ShouldEndSession = false,
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>I am sorry, I can not shuffle Radio-T podcasts, they are hours long</speak>"
                    }
                }
            };
            return response;
        }
    }
}