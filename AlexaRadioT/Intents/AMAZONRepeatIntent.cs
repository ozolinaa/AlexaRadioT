using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Intents
{
    public class AMAZONRepeatIntent : IAlexaIntent
    {
        public virtual AlexaResponse ProcessRequest(AlexaRequest request)
        {
            AlexaUserModel user = User.GetById(request.Context.System.User.UserId);

            string token = request.Context.AudioPlayer.Token;
            long offsetInMilliseconds = request.Context.AudioPlayer.OffsetInMilliseconds;

            if (user.ListeningAudioToken.Equals("LiveStream", StringComparison.OrdinalIgnoreCase))
            {
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

            int podcastNumber = Int32.Parse(token);
            long newOffset = offsetInMilliseconds - 10 * 1000;
            newOffset = newOffset > 0 ? newOffset : 0;

            return new AlexaResponse()
            {
                Session = null,
                Response = new AlexaResponse.ResponseAttributes()
                {
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(podcastNumber), newOffset, podcastNumber.ToString())
                    }
                }
            };


        }
    }
}
