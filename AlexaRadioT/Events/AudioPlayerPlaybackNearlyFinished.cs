using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;

namespace AlexaRadioT.Events
{
    public class AudioPlayerPlaybackNearlyFinished : IAlexaEvent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            return null;
        }
    }
}