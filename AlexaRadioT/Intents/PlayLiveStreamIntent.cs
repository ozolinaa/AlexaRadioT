using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

namespace AlexaRadioT.Intents
{
    public class PlayLiveStreamIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            TimeSpan duration = RadioT.HowLongForNextLiveStream();
            if (duration.TotalMinutes > 0)
                return RespondWhenNext(request, duration);

            User.SaveListenPosition(request.Session.User.UserId, "LiveStream", 0);
            return new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.Format("<speak>{0}</speak>", "Playing <phoneme ph=\"laɪv\">live</phoneme> stream")
                    },
                    Card = new AlexaResponse.ResponseAttributes.CardAttributes()
                    {
                        Type = "Simple",
                        Title = "Radio-T ",
                        Content = "Playing Live Stream "
                    },
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForLiveStream(), 0, "LiveStream")
                    }
                }
            };  
        }

        private AlexaResponse RespondWhenNext(AlexaRequest request, TimeSpan duration)
        {
            AlexaResponse response = new AlexaResponse();
            response.Response.OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
            {
                Type = "SSML",
                Ssml = string.Format("<speak>{0} {1}</speak>",
                    "Next Radio-T will be <phoneme ph=\"laɪv\">live</phoneme> in",
                    Speech.ToString(duration, applySSML: true))
            };
            response.Response.Card = new AlexaResponse.ResponseAttributes.CardAttributes()
            {
                Type = "Simple",
                Title = "Next Radio-T",
                Content = "Will be live in " + Speech.ToString(duration)
            };

            return response;
        }

    }
}