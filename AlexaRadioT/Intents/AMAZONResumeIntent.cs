using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

namespace AlexaRadioT.Intents
{
    public class AMAZONResumeIntent : IAlexaIntent
    {
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            AlexaUserModel user = User.GetById(request.Session.User.UserId);

            Log.LogDebug(string.Format("User.GetById Id {0} ListeningAudioToken {1} OffsetInMilliseconds {2}", user.Id, user.ListeningAudioToken, user.OffsetInMilliseconds));

            if (string.IsNullOrEmpty(user.ListeningAudioToken))
                return null;

            if (Int32.TryParse(user.ListeningAudioToken, out int podcastNumber))
                return _resumePodcast(request, podcastNumber, user.OffsetInMilliseconds.Value);
            else if (user.ListeningAudioToken.Equals("LiveStream", StringComparison.OrdinalIgnoreCase))
                return _resumeLiveStream(request);
            else
                return null;
        }

        private AlexaResponse _resumePodcast(AlexaRequest request, int podcastNumber, long offset)
        {
            PodcastEnity podcast = RadioT.GetPodcastDetails(podcastNumber);

            Log.LogDebug(string.Format("Resuming podcastNumber {0} offset {1}", podcastNumber, offset));

            return new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.Format("<speak>{0} {1}</speak>",
                            "Resuming Radio-T podcast number",
                            podcastNumber)
                    },
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(podcastNumber), offset, podcastNumber.ToString())
                    }
                }
            };
        }

        private AlexaResponse _resumeLiveStream(AlexaRequest request)
        {
            return new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>Resuming Radio-T <phoneme ph=\"laɪv\">live</phoneme> stream</speak>"
                    },
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForLiveStream(), 0, "LiveStream")
                    }
                }
            };
        }

    }
}