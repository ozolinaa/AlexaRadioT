using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Intents
{
    public class AMAZONStartOverIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            string listeningAudioToken = request.Context.AudioPlayer.Token;

            if (listeningAudioToken.Equals("LiveStream", StringComparison.OrdinalIgnoreCase)) {
                return new AlexaResponse()
                {
                    Response = new AlexaResponse.ResponseAttributes()
                    {
                        ShouldEndSession = true,
                        OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                        {
                            Type = "SSML",
                            Ssml = "<speak>I can not do that, this is <phoneme ph=\"laɪv\">live</phoneme> stream</speak>"
                        }
                    }
                };
            }

            int podcastNumber = Int32.Parse(listeningAudioToken);

            AlexaUserModel user = User.GetById(request.Context.System.User.UserId);
            User.SaveListenPosition(user.Id, listeningAudioToken, 0);

            return new AlexaResponse()
            {
                Session = null,
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>Starting podcast number " + listeningAudioToken + " </speak>"
                    },
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(podcastNumber), 0, podcastNumber.ToString())
                    }
                }
            };
        }
    }
}
