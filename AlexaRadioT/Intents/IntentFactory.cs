using AlexaRadioT.Intents.Amazon;
using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AlexaRadioT.Intents
{
    public class IntentFactory
    {
        public static IAlexaIntent GetIntentForRequest(AlexaRequest request)
        {
            string requestedIntentName = null;

            if (request != null && request.Request != null && request.Request.Intent != null && request.Request.Intent.Name != null)
            {
                requestedIntentName = request.Request.Intent.Name.Replace(".", "");
            }

            if (string.IsNullOrEmpty(requestedIntentName))
                return null;

            IAlexaIntent intent = null;
            Type Itype = typeof(IAlexaIntent);
            Type t = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => Itype.IsAssignableFrom(p) && p != Itype)
                .FirstOrDefault(x => x.Name.Equals(requestedIntentName, StringComparison.OrdinalIgnoreCase));

            if (t != null)
            {
                intent = (IAlexaIntent)Activator.CreateInstance(t);
            }
            else
            {
                intent = new AMAZONHelpIntent(); //unknown user intent - ask user
            }

            return intent;
        }
    }
}