using System;
using Microsoft.AspNetCore.Mvc;
using AlexaRadioT.Store;
using System.IO;
using AlexaRadioT.Models;
using Newtonsoft.Json;
using AlexaRadioT.Intents;
using AlexaRadioT.Events;

namespace AlexaRadioT.Controllers
{
    [Route("api/[controller]")]
    public class AlexaController : Controller
    {
        [HttpPost]
        public AlexaResponse IntentAction()
        {
            AlexaResponse response = null;

            try
            {
                string requestBodyStr = null;
                using (StreamReader reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8))
                {
                    requestBodyStr = reader.ReadToEndAsync().Result;
                }

                Guid loggedRequestId = Log.LogAlexaRequest(requestBodyStr);
                AlexaRequest request = JsonConvert.DeserializeObject<AlexaRequest>(requestBodyStr);
                IAlexaIntent intent = IntentFabric.GetIntentForRequest(request);
                if (intent != null)
                {
                    response = intent.ProcessRequest(request);
                }
                else
                {
                    IAlexaEvent aEvent = EventFabric.GetEventForRequest(request);
                    if (aEvent != null)
                    {
                        response = aEvent.ProcessRequest(request);
                    }
                }
                Log.LogAlexaResponse(loggedRequestId, response);
            }
            catch (Exception e)
            {
                Log.LogException(e);
            }

            if (response == null) {
                response = new AlexaResponse() { Session = null };
            }

            return response;
        }
    }
}
