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
        public virtual AlexaResponse ProcessRequest(AlexaRequest request)
        {
            return _resume(request, true);
        }

        protected AlexaResponse _resume(AlexaRequest request, bool withVoice = true)
        {
            AlexaUserModel user = User.GetById(request.Context.System.User.UserId);

            Log.LogDebug(string.Format("User.GetById Id {0} ListeningAudioToken {1} OffsetInMilliseconds {2}", user.Id, user.ListeningAudioToken, user.OffsetInMilliseconds));

            if (string.IsNullOrEmpty(user.ListeningAudioToken))
                return null;

            if (Int32.TryParse(user.ListeningAudioToken, out int podcastNumber))
                return _resumePodcast(request, podcastNumber, user.OffsetInMilliseconds.Value, withVoice);
            else if (user.ListeningAudioToken.Equals("LiveStream", StringComparison.OrdinalIgnoreCase))
                return _resumeLiveStream(request, withVoice);
            else
                return null;
        }

        protected AlexaResponse _resumePodcast(AlexaRequest request, int podcastNumber, long offset, bool withVoice)
        {
            PodcastEnity podcast = RadioT.GetPodcastDetails(podcastNumber);

            Log.LogDebug(string.Format("Resuming podcastNumber {0} offset {1}", podcastNumber, offset));

            return new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = withVoice ? new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = string.Format("<speak>{0} {1}</speak>",
                            "Resuming Radio-T podcast number",
                            podcastNumber)
                    } : null,
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(podcastNumber), offset, podcastNumber.ToString())
                    }
                }
            };
        }

        protected AlexaResponse _resumeLiveStream(AlexaRequest request, bool withVoice)
        {
            return new AlexaResponse()
            {
                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = withVoice ? new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>Resuming Radio-T <phoneme ph=\"laɪv\">live</phoneme> stream</speak>"
                    } : null,
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForLiveStream(), 0, "LiveStream")
                    }
                }
            };
        }

    }
}