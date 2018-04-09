using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexaRadioT.Events
{
    public interface IAlexaEvent
    {
        void ProcessRequest(Models.AlexaRequest request);
    }
}
