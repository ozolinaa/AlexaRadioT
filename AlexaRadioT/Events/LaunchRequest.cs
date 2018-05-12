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
            // Try Play Live Stream
            TimeSpan duration = RadioT.HowLongForNextLiveStream();
            if (duration.TotalMilliseconds <= 0)
                return PlayLiveStream(request);

            // Try Resume
            AlexaUserModel user = User.GetById(request.Context.System.User.UserId);
            if (string.IsNullOrEmpty(user.ListeningAudioToken) == false)
                if (Int32.TryParse(user.ListeningAudioToken, out int podcastNumber))
                    return new AMAZONResumeIntent().ProcessRequest(request);

            // Play Last Podcast
            PodcastEnity podcast = RadioT.GetLatestPodcasts(1).First();
            return PlayPodcastNumberIntent.PlayPodcast(request, podcast.Number, "<speak>Welcome to Radio-T! Playing latest podcast</speak>");
        }


        private AlexaResponse PlayLiveStream(AlexaRequest request)
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
