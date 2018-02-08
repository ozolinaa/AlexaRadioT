using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

namespace AlexaRadioT.Intents
{
    public class AMAZONStopIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            User.ClearListenPosition(request.Session.User.UserId);

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
