using AlexaRadioT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexaRadioT.Events
{
    public interface IAlexaEvent
    {
        AlexaResponse ProcessRequest(Models.AlexaRequest request);
    }
}
