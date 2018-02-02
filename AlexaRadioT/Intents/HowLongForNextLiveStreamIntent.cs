﻿using System;
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

            if (duration.TotalSeconds > 0)
            {
                return RespondWhenNext(request, duration);
            }
            else
            {
                return RespondItsLive(request, duration);
            }

        }

        private AlexaResponse RespondWhenNext(AlexaRequest request, TimeSpan duration) {
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

        private AlexaResponse RespondItsLive(AlexaRequest request, TimeSpan duration)
        {
            AlexaResponse response = new AlexaResponse();
            response.Response.OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
            {
                Ssml = string.Format("<speak>{0} {1} {2}</speak>",
                    "Radio-T is <phoneme ph=\"laɪv\">live</phoneme> now, it's playing for",
                    Speech.ToString(duration, applySSML: true),
                    "Would you like to play <phoneme ph=\"laɪv\">live</phoneme> stream now?")
            };

            return response;
        }
    }
}