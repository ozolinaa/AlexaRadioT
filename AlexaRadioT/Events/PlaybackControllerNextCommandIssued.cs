using AlexaRadioT.Intents;
using AlexaRadioT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Events
{
    public class PlaybackControllerNextCommandIssued : PlayNextTopicIntent, IAlexaEvent
    {
        public override AlexaResponse ProcessRequest(AlexaRequest request)
        {
            return _respondWithNextTopic(request, false);
        }
    }
}
