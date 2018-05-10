using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;

namespace AlexaRadioT.Intents
{
    public class AMAZONHelpIntent : IAlexaIntent
    {
        Dictionary<string, string> availableIntentsDescriptions = new Dictionary<string, string>()
        {
            {"To play last podcast","To play last podcast"},
            {"To play live stream","To play <phoneme ph=\"laɪv\">live</phoneme> stream"},
            {"When will be the next show","When will be the next show"}
        };

        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            AlexaResponse response = new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    ShouldEndSession = false,
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.Format("<speak>You can ask me the following: {0} {1}</speak>", 
                            string.Join(" ", availableIntentsDescriptions.Values.Select(x=> "<p>" + x + "</p>")),
                            "What would you like me to do?")
                    },
                    Reprompt = new AlexaResponse.ResponseAttributes.RepromptAttributes()
                    {
                        OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                        {
                            Type = "SSML",
                            Ssml = string.Format("<speak>You can ask me the following: {0} {1}</speak>",
                            string.Join(" ", availableIntentsDescriptions.Values.Select(x => "<p>" + x + "</p>")),
                            "What would you like me to do?")
                        }
                    },
                    Card = new AlexaResponse.ResponseAttributes.CardAttributes()
                    {
                        Type = "Simple",
                        Title = "Radio-T Help",
                        Content = string.Format("You can ask me {0}",
                        string.Join("\r\n", availableIntentsDescriptions.Keys))
                    }
                }
            };
            return response;
        }
    }
}