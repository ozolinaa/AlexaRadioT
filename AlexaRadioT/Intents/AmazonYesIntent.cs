using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlexaRadioT.Models;

namespace AlexaRadioT.Intents
{
    public class AMAZONYesIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            if (string.IsNullOrEmpty(request.Session.Attributes.NextIntentName) == false)
            {
                request.Request.Intent.Name = request.Session.Attributes.NextIntentName;
                request.Request.Intent.Name = typeof(HowLongForNextLiveStreamIntent).ToString();
                request.Session.Attributes.NextIntentName = "";
                IAlexaIntent intent = IntentFactory.GetIntentForRequest(request);
                if (intent != null)
                    return intent.ProcessRequest(request);
            }
            return null;
        }
    }
}
