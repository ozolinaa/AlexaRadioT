using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;
using AlexaRadioT.Store;
using AlexaRadioT.Intents.Amazon;

namespace AlexaRadioT.Intents
{
    public class PlayPodcastNumberIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            string numberStr = request.Request.Intent.GetSlotValue("Number");
            if (string.IsNullOrEmpty(numberStr))
            {
                AMAZONHelpIntent help = new AMAZONHelpIntent();
                return help.ProcessRequest(request);
            }

            int number = Int32.Parse(numberStr);
            return PlayPodcast(request, number);
        }

        public static AlexaResponse PlayPodcast(AlexaRequest request, int podcastNumber, string customSsml = null)
        {
            User.GetById(request.Session.User.UserId); //create user if missing
            User.SaveListenPosition(request.Session.User.UserId, podcastNumber.ToString(), 0);

            return new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.IsNullOrEmpty(customSsml) ? string.Format("<speak>{0} {1}</speak>",
                            "Playing Radio-T podcast number",
                            podcastNumber) : customSsml
                    },
                    Card = new AlexaResponse.ResponseAttributes.CardAttributes()
                    {
                        Type = "Simple",
                        Title = "Radio-T ",
                        Content = "Playing the podcast " + podcastNumber
                    },
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(podcastNumber), 0, podcastNumber.ToString())
                    }
                }
            };
        }
    }
}