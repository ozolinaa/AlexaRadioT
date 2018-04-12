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

        private PodcastTimeLabel _getNextTopic(PodcastEnity podcast, long offsetInMilliseconds)
        {
            if (podcast.OrderedTimeLabels == null)
                return null;

            PodcastTimeLabel nextTopic = null;
            foreach (PodcastTimeLabel timeLabel in podcast.OrderedTimeLabels)
            {
                if ((timeLabel.Time - podcast.StartDateTime).TotalMilliseconds > offsetInMilliseconds)
                {
                    nextTopic = timeLabel;
                    break;
                }
            }

            return nextTopic;
        }


        protected AlexaResponse _respondWithNextTopic(AlexaRequest request, bool withVoice)
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
            PodcastEnity podcast = RadioT.GetPodcastDetails(podcastNumber);
            PodcastTimeLabel nextTopic = _getNextTopic(podcast, offsetInMilliseconds);

            if (nextTopic == null)
            {
                try
                {
                    PodcastEnity nextPodcast = RadioT.GetPodcastDetails(podcast.Number + 1);
                    if (nextPodcast == null)
                        throw new Exception("no next podcast");

                    return new AlexaResponse()
                    {
                        Session = null,
                        Response = new AlexaResponse.ResponseAttributes()
                        {
                            OutputSpeech = withVoice ? new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                            {
                                Type = "SSML",
                                Ssml = "<speak>Starting podcast number " + nextPodcast.Number + " </speak>"
                            } : null,
                            Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                                new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(nextPodcast.Number), 0, nextPodcast.Number.ToString())
                            }
                        }
                    };
                }
                catch (Exception)
                {
                    return new AlexaResponse()
                    {
                        Session = null,
                        Response = new AlexaResponse.ResponseAttributes()
                        {
                            OutputSpeech = withVoice ? new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                            {
                                Type = "SSML",
                                Ssml = "<speak>You have listened the last Radio-T podcast</speak>"
                            } : null,
                            Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                                new AlexaResponse.ResponseAttributes.AudioDirective("Stop")
                            }
                        }
                    };
                }
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