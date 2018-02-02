using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexaRadioT.Events
{
    public interface IAlexaEvent
    {
        Models.AlexaResponse ProcessRequest(Models.AlexaRequest request);
    }
}
