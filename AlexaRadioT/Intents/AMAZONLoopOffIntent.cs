using AlexaRadioT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaRadioT.Intents
{
    public class AMAZONLoopOffIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            AlexaResponse response = new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    ShouldEndSession = true,
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>Loop is turned off</speak>"
                    }
                }
            };
            return response;
        }
    }
}