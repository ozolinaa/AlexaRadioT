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

            return PlayPodcastNumber(request, number);
        }

        public static AlexaResponse PlayPodcastNumber(AlexaRequest request, int number)
        {
            User.GetById(request.Session.User.UserId); //create user if missing
            User.SaveListenPosition(request.Session.User.UserId, number.ToString(), 0);
            return new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.Format("<speak>{0} {1}</speak>",
                            "Playing Radio-T podcast number",
                            number)
                    },
                    Card = new AlexaResponse.ResponseAttributes.CardAttributes()
                    {
                        Type = "Simple",
                        Title = "Radio-T ",
                        Content = "Playing the podcast " + number
                    },
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(number), 0, number.ToString())
                    }
                }
            };
        }
    }
}