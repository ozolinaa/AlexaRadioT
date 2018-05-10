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
            AlexaResponse response = new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.Format("<speak>{0} {1} {2}</speak>",
                    "<phoneme ph=\"laɪv\">Live</phoneme> stream is not available right now, the next show will be <phoneme ph=\"laɪv\">live</phoneme> in",
                    Speech.ToString(duration, applySSML: true),
                    "Would you like to listen to the latest available episode? Yes or No?")
                    },
                    Reprompt = new AlexaResponse.ResponseAttributes.RepromptAttributes()
                    {
                        OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                        {
                            Type = "SSML",
                            Ssml = string.Format("<speak>{0}</speak>",
                            "Would you like to play the last Radio-T episode? Yes or No?")
                        }
                    },
                    Card = new AlexaResponse.ResponseAttributes.CardAttributes()
                    {
                        Type = "Simple",
                        Title = "Not on-air right now",
                        Content = "Radio-T be live in " + Speech.ToString(duration)
                    },
                    ShouldEndSession = false,
                },
                Session = new AlexaRequest.SessionCustomAttributes()
                {
                    NextIntentName = typeof(PlayLastPodcastIntent).Name
                }
            };

            return response;
        }

    }
}