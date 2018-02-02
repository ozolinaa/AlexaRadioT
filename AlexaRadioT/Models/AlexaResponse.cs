using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Models
{
    [JsonObject]
    public class AlexaResponse
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("sessionAttributes")]
        public AlexaRequest.SessionCustomAttributes Session { get; set; }

        [JsonProperty("response")]
        public ResponseAttributes Response { get; set; }

        public AlexaResponse()
        {
            Version = "1.0";
            Session = new AlexaRequest.SessionCustomAttributes();
            Response = new ResponseAttributes();
        }

        [JsonObject("response")]
        public class ResponseAttributes
        {
            [JsonProperty("shouldEndSession")]
            public bool ShouldEndSession { get; set; }

            [JsonProperty("outputSpeech")]
            public OutputSpeechAttributes OutputSpeech { get; set; }

            [JsonProperty("card")]
            public CardAttributes Card { get; set; }

            [JsonProperty("reprompt")]
            public RepromptAttributes Reprompt { get; set; }

            public ResponseAttributes()
            {
                ShouldEndSession = true;
                OutputSpeech = null;
                Card = null;
                Reprompt = null;
            }

            [JsonObject("outputSpeech")]
            public class OutputSpeechAttributes
            {
                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("text")]
                public string Text { get; set; }

                [JsonProperty("ssml")]
                public string Ssml { get; set; }

                public OutputSpeechAttributes()
                {
                    Type = "PlainText";
                }
            }

            [JsonObject("card")]
            public class CardAttributes
            {
                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("title")]
                public string Title { get; set; }

                [JsonProperty("content")]
                public string Content { get; set; }

                public CardAttributes()
                {
                    Type = "Simple";
                }
            }

            [JsonObject("reprompt")]
            public class RepromptAttributes
            {
                [JsonProperty("outputSpeech")]
                public OutputSpeechAttributes OutputSpeech { get; set; }

                public RepromptAttributes()
                {
                    OutputSpeech = new OutputSpeechAttributes();
                }
            }

            [JsonProperty("directives")]
            public AudioDirective[] Directives { get; set; }

            public class AudioDirective
            {
                public AudioDirective()
                {
                    Type = "AudioPlayer";
                }

                public AudioDirective(string type) : this()
                {
                    Type = Type + "." + type;
                }

                public AudioDirective(Uri audioUrl, double offsetInMilliseconds = 0, string token = "") : this("Play")
                {
                    PlayBehavior = "REPLACE_ALL";
                    AudioItem = new AudioItemAttributes()
                    {
                        Stream = new AudioItemAttributes.StreamAttributes()
                        {
                            Url = audioUrl,
                            OffsetInMilliseconds = offsetInMilliseconds,
                            Token = string.IsNullOrEmpty(token) ? audioUrl.ToString() : token
                        }
                    };
                }

                [JsonProperty("type")]
                public string Type { get; set; }
                [JsonProperty("playBehavior")]
                public string PlayBehavior { get; set; }
                [JsonProperty("audioItem")]
                public AudioItemAttributes AudioItem { get; set; }

                public class AudioItemAttributes
                {
                    [JsonProperty("stream")]
                    public StreamAttributes Stream { get; set; }

                    public class StreamAttributes
                    {
                        [JsonProperty("url")]
                        public Uri Url { get; set; }
                        [JsonProperty("token")]
                        public string Token { get; set; }
                        [JsonProperty("offsetInMilliseconds")]
                        public double OffsetInMilliseconds { get; set; }
                    }
                }
            }


        }
    }
}
