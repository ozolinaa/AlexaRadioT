using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlexaRadioT.Models
{
    public class PodcastEnity
    {
        [JsonProperty("url")]
        public string URL { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        public int Number
        {
            get
            {
                return Convert.ToInt32(Regex.Replace(Title, "[^0-9]", ""));
            }
        }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("categories")]
        public string[] Categories { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("file_name")]
        public string FileName { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("show_notes")]
        public string ShowNotes { get; set; }
        [JsonProperty("audio_url")]
        public string AudioURL { get; set; }


        [JsonProperty("time_labels")]
        public PodcastTimeLabel[] TimeLabels { get; set; }

        public IOrderedEnumerable<PodcastTimeLabel> OrderedTimeLabels
        {
            get
            {
                if (TimeLabels != null && TimeLabels.Length > 0)
                {
                    return TimeLabels.OrderBy(x => x.Time);
                }
                return null;
            }
        }

        public DateTime StartDateTime
        {
            get
            {
                if (TimeLabels != null && TimeLabels.Any())
                {
                    DateTime start = TimeLabels[0].Time;
                    return start.AddMinutes(start.Minute * -1);
                }
                else
                {
                    return DateTime.MinValue;
                }
                
            }
        }



        public PodcastTimeLabel GetNextTopic(long currentOffsetInMilliseconds)
        {
            if (OrderedTimeLabels == null)
                return null;

            PodcastTimeLabel nextTopic = null;
            foreach (PodcastTimeLabel timeLabel in OrderedTimeLabels)
            {
                if ((timeLabel.Time - StartDateTime).TotalMilliseconds > currentOffsetInMilliseconds)
                {
                    nextTopic = timeLabel;
                    break;
                }
            }

            return nextTopic;
        }


        public PodcastTimeLabel GetPreviousTopic(long currentOffsetInMilliseconds)
        {
            if (OrderedTimeLabels == null || OrderedTimeLabels.Any() == false)
                return null;

            PodcastTimeLabel currentTopic = OrderedTimeLabels.First();
            foreach (PodcastTimeLabel timeLabel in OrderedTimeLabels)
            {
                if ((timeLabel.Time - StartDateTime).TotalMilliseconds < currentOffsetInMilliseconds)
                {
                    currentTopic = timeLabel;
                }
            }

            PodcastTimeLabel previousTopic = null;
            foreach (PodcastTimeLabel timeLabel in OrderedTimeLabels)
            {
                if (timeLabel.Time.Ticks == currentTopic.Time.Ticks)
                {
                    if (previousTopic != null && previousTopic.Time.Ticks == timeLabel.Time.Ticks)
                    {
                        previousTopic = null;
                    }
                    break;
                }
                previousTopic = timeLabel;
            }

            return previousTopic;
        }

    }
}
