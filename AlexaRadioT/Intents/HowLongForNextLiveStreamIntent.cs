using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

namespace AlexaRadioT.Intents
{
    public class HowLongForNextLiveStreamIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            TimeSpan duration = RadioT.HowLongForNextLiveStream();
            if (duration.TotalMilliseconds > 0)
                return RespondWhenNext(request, duration);
            else
                return RespondItsLive(request, (duration * -1));
        }

        private AlexaResponse RespondWhenNext(AlexaRequest request, TimeSpan duration) {
            AlexaResponse response = new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.Format("<speak>{0} {1} {2}</speak>",
                    "Next Radio-T will be <phoneme ph=\"laɪv\">live</phoneme> in",
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
                        Title = "Next Radio-T",
                        Content = "Will be live in " + Speech.ToString(duration)
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

        private AlexaResponse RespondItsLive(AlexaRequest request, TimeSpan elapsed)
        {
            AlexaResponse response = new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.Format("<speak>{0} {1}</speak>",
                    "Radio-T is <phoneme ph=\"laɪv\">live</phoneme> now, ",
                    "would you like to listen it? Yes or No?")
                    },
                    Reprompt = new AlexaResponse.ResponseAttributes.RepromptAttributes()
                    {
                        OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                        {
                            Type = "SSML",
                            Ssml = string.Format("<speak>{0}</speak>",
                            "Would you like to listen Radio-T <phoneme ph=\"laɪv\">live</phoneme> stream now? Yes or No?")
                        }
                    },
                    ShouldEndSession = false
                },
                Session = new AlexaRequest.SessionCustomAttributes()
                {
                    NextIntentName = typeof(PlayLiveStreamIntent).Name
                }
            };

            return response;
        }
    }
}