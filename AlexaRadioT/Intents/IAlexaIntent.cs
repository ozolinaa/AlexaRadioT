using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexaRadioT.Intents
{
    public interface IAlexaIntent
    {
        Models.AlexaResponse ProcessRequest(Models.AlexaRequest request);
    }
}
