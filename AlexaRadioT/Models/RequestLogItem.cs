using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Models
{
    public class LogItem
    {
        public Guid ID { get; set; }
        public DateTime LoggedDateTime { get; set; }
        public string Text { get; set; }
    }
}
