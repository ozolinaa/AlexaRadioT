using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Models
{
    [JsonObject]
    public class AlexaRequest
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("session")]
        public SessionAttributes Session { get; set; }

        [JsonProperty("request")]
        public RequestAttributes Request { get; set; }

        [JsonProperty("context")]
        public ContextAttributes Context { get; set; }

        [JsonObject("attributes")]
        public class SessionCustomAttributes
        {
            [JsonProperty("nextIntentName")]
            public string NextIntentName { get; set; }

            [JsonProperty("nextIntentSlotsJson")]
            public string NextIntentSlotsJson { get; set; }
        }

        [JsonObject("session")]
        public class SessionAttributes
        {
            [JsonProperty("sessionId")]
            public string SessionId { get; set; }

            [JsonProperty("application")]
            public ApplicationAttributes Application { get; set; }

            [JsonProperty("attributes")]
            public SessionCustomAttributes Attributes { get; set; }

            [JsonProperty("user")]
            public UserAttributes User { get; set; }

            [JsonProperty("new")]
            public bool New { get; set; }

            [JsonObject("application")]
            public class ApplicationAttributes
            {
                [JsonProperty("applicationId")]
                public string ApplicationId { get; set; }
            }

            [JsonObject("user")]
            public class UserAttributes
            {
                [JsonProperty("userId")]
                public string UserId { get; set; }

                [JsonProperty("accessToken")]
                public string AccessToken { get; set; }
            }
        }

        [JsonObject("request")]
        public class RequestAttributes
        {
            private string _timestampEpoch;
            private double _timestamp;

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("requestId")]
            public string RequestId { get; set; }

            [JsonProperty("timestamp")]
            public string TimestampEpoch
            {
                get
                {
                    return _timestampEpoch;
                }
                set
                {
                    _timestampEpoch = value;

                    if (Double.TryParse(value, out _timestamp) && _timestamp > 0)
                        Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(_timestamp);
                    else
                    {
                        var timeStamp = DateTime.MinValue;
                        if (DateTime.TryParse(_timestampEpoch, out timeStamp))
                            Timestamp = timeStamp.ToUniversalTime();
                    }
                }
            }

            [JsonIgnore]
            public DateTime Timestamp { get; set; }

            [JsonProperty("intent")]
            public IntentAttributes Intent { get; set; }

            [JsonProperty("reason")]
            public string Reason { get; set; }

            public RequestAttributes()
            {
                Intent = new IntentAttributes();
            }

            [JsonObject("intent")]
            public class IntentAttributes
            {
                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("slots")]
                public dynamic Slots { get; set; }

                public List<KeyValuePair<string, string>> GetSlots()
                {
                    var output = new List<KeyValuePair<string, string>>();
                    if (Slots == null) return output;

                    foreach (var slot in Slots.Children())
                    {
                        if (slot.First.value != null)
                            output.Add(new KeyValuePair<string, string>(slot.First.name.ToString(), slot.First.value.ToString()));
                    }

                    return output;
                }

                public string GetSlotValue(string key)
                {
                    KeyValuePair<string, string> slot = GetSlots().FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    return slot.Value;
                }
            }


        }

        [JsonObject("context")]
        public class ContextAttributes
        {
            [JsonProperty("AudioPlayer")]
            public AudioPlayerContext AudioPlayer { get; set; }

            [JsonProperty("System")]
            public SystemAttributes System { get; set; }

            [JsonObject("AudioPlayer")]
            public class AudioPlayerContext
            {
                [JsonProperty("offsetInMilliseconds")]
                public long OffsetInMilliseconds { get; set; }
                [JsonProperty("token")]
                public string Token { get; set; }
                [JsonProperty("playerActivity")]
                public string PlayerActivity { get; set; }
            }

            [JsonObject("System")]
            public class SystemAttributes
            {
                [JsonProperty("application")]
                public SessionAttributes.ApplicationAttributes Application { get; set; }

                [JsonProperty("user")]
                public SessionAttributes.UserAttributes User { get; set; }
            }
        }
    }
}
