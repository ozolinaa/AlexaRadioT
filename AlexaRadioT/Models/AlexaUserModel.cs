using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Models
{
    public class AlexaUserModel
    {
        public string Id { get; set; }
        public string ListeningAudioToken { get; set; }
        public long? OffsetInMilliseconds { get; set; }
    }
}
