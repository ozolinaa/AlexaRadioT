using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlexaRadioT.Intents
{
    public class PlayPreviousTopicIntent : IAlexaIntent
    {
        public virtual AlexaResponse ProcessRequest(AlexaRequest request)
        {
            return _respondWithPreviousTopic(request, true);
        }


        protected AlexaResponse _respondWithPreviousTopic(AlexaRequest request, bool withVoice)
        {
            AlexaUserModel user = User.GetById(request.Context.System.User.UserId);

            string token = request.Context.AudioPlayer.Token;

            if (token.Equals("LiveStream", StringComparison.OrdinalIgnoreCase))
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
            long offsetInMilliseconds = request.Context.AudioPlayer.OffsetInMilliseconds;
            PodcastEnity podcast = RadioT.GetPodcastDetails(podcastNumber);
            PodcastTimeLabel previousTopic = podcast.GetPreviousTopic(offsetInMilliseconds);

            if (previousTopic == null)
            {
                int previousPodcastNumber = podcast.Number - 1;

                if (previousPodcastNumber <= 0)
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
                                Ssml = "<speak>You have listened the first podcast</speak>"
                            } : null,
                            Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                                new AlexaResponse.ResponseAttributes.AudioDirective("Stop")
                            }
                        }
                    };
                }

                User.SaveListenPosition(user.Id, previousPodcastNumber.ToString(), 0);

                return new AlexaResponse()
                {
                    Session = null,
                    Response = new AlexaResponse.ResponseAttributes()
                    {
                        OutputSpeech = withVoice ? new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                        {
                            Type = "SSML",
                            Ssml = "<speak>Starting podcast number " + previousPodcastNumber + " </speak>"
                        } : null,
                        Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                            new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(previousPodcastNumber), 0, previousPodcastNumber.ToString())
                        }
                    }
                };
            }

            long newOffset = previousTopic.GetOffset(podcast.StartDateTime);
            User.SaveListenPosition(user.Id, user.ListeningAudioToken, newOffset);
            return new AlexaResponse()
            {

                Response = new AlexaResponse.ResponseAttributes()
                {
                    OutputSpeech = withVoice ? new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>Playing previous news</speak>"
                    } : null,
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(podcast.Number), newOffset, podcast.Number.ToString())
                    }
                }
            };

        }
    }
}