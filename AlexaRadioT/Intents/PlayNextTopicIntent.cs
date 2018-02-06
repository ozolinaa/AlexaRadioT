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
        public AlexaResponse ProcessRequest(AlexaRequest request)
        {
            AlexaUserModel user = User.GetById(request.Session.User.UserId);
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
                        OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                        {
                            Type = "SSML",
                            Ssml = "<speak>You have listened the last topic</speak>"
                        },
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
                    OutputSpeech = new AlexaResponse.ResponseAttributes.OutputSpeechAttributes()
                    {
                        Type = "SSML",
                        Ssml = "<speak>Playing next news</speak>"
                    },
                    Directives = new AlexaResponse.ResponseAttributes.AudioDirective[] {
                        new AlexaResponse.ResponseAttributes.AudioDirective(RadioT.GetUriForPodcast(podcast), newOffset, podcast.Number.ToString())
                    }
                }
            };

        }
    }
}