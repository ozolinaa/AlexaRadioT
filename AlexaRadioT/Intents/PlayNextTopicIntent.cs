using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaRadioT.Intents
{
    public class PlayNextTopicIntent : IAlexaIntent
    {
        public virtual AlexaResponse ProcessRequest(AlexaRequest request)
        {
            return _respondWithNextTopic(request, true);
        }

        protected AlexaResponse _respondWithNextTopic(AlexaRequest request, bool withVoice)
        {
            AlexaUserModel user = User.GetById(request.Context.System.User.UserId);

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

            PodcastEnity podcast = RadioT.GetPodcastDetails(Int32.Parse(user.ListeningAudioToken));

            PodcastTimeLabel nextTopic = null;
            foreach (PodcastTimeLabel timeLabel in podcast.OrderedTimeLabels)
            {
                if ((timeLabel.Time - podcast.StartDateTime).TotalMilliseconds > user.OffsetInMilliseconds)
                {
                    nextTopic = timeLabel;
                    break;
                }
            }

            if (nextTopic == null)
            {
                User.ClearListenPosition(user.Id);
                return new AlexaResponse()
                {
                    Session = null,
                    Response = new AlexaResponse.ResponseAttributes()
                    {
                        OutputSpeech = withVoice ? new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                        {
                            Type = "SSML",
                            Ssml = "<speak>You have listened the last topic</speak>"
                        } : null,
                        Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                            new AlexaResponse.ResponseAttributes.AudioDirective("Stop")
                        }
                    }
                };
            }

            long newOffset = nextTopic.GetOffset(podcast.StartDateTime);
            User.SaveListenPosition(user.Id, user.ListeningAudioToken, newOffset);
            return new AlexaResponse()
            {

                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = withVoice ? new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>Playing next news</speak>"
                    } : null,
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(podcast.Number), newOffset, podcast.Number.ToString())
                    }
                }
            };

        }
    }
}