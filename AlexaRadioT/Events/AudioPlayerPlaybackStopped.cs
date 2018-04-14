using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

namespace AlexaRadioT.Events
{
    public class AudioPlayerPlaybackStopped : IAlexaEvent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            User.SaveListenPosition(request.Context.System.User.UserId, request.Context.AudioPlayer.Token, request.Context.AudioPlayer.OffsetInMilliseconds);

            return null;
        }
    }
}