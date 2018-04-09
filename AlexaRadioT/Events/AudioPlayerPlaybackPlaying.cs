using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

namespace AlexaRadioT.Events
{
    public class AudioPlayerPlaybackPlaying : IAlexaEvent
    {
        public void ProcessRequest(AlexaRequest request)
        {
            Log.LogDebug(string.Format("AudioPlayerPlaybackPlaying Id {0} audioToken {1} offsetInMilliseconds {2}", request.Context.System.User.UserId, request.Context.AudioPlayer.Token, request.Context.AudioPlayer.OffsetInMilliseconds));

            User.SaveListenPosition(request.Context.System.User.UserId, request.Context.AudioPlayer.Token, request.Context.AudioPlayer.OffsetInMilliseconds);
        }
    }
}