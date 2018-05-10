using AlexaRadioT.Intents;
using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Events
{
    public class LaunchRequest : IAlexaEvent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            TimeSpan duration = RadioT.HowLongForNextLiveStream();
            if (duration.TotalMilliseconds > 0)
            {
                PodcastEnity podcast = RadioT.GetLatestPodcasts(1).First();
                return PlayPodcastNumberIntent.PlayPodcast(request, podcast.Number, "<speak>Welcome to Radio-T! Playing latest podcast</speak>");
            }
            else
            {
                User.SaveListenPosition(request.Session.User.UserId, "LiveStream", 0);
                return new AlexaResponse()
                {
                    Response = new AlexaResponse.ResponseAttributes()
                    {
                        OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                        {
                            Type = "SSML",
                            Ssml = string.Format("<speak>{0}</speak>", "Welcome to Radio-T! Playing <phoneme ph=\"laɪv\">live</phoneme> stream")
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
        }
    }
}
