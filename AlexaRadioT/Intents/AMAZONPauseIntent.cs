using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;

namespace AlexaRadioT.Intents
{
    public class AMAZONPauseIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            return new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective("Stop")
                    }
                }
            };
        }
    }
}