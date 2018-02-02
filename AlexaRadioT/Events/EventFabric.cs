using AlexaRadioT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexaRadioT.Events
{
    public class EventFabric
    {
        public static IAlexaEvent GetEventForRequest(AlexaRequest request)
        {
            string requestedEvenTypeName = request.Request.Type != null ? request.Request.Type.Replace(".", "") : "";
            if (string.IsNullOrEmpty(requestedEvenTypeName))
                return null;

            IAlexaEvent aEvent = null;
            Type Itype = typeof(IAlexaEvent);
            Type t = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => Itype.IsAssignableFrom(p) && p != Itype)
                .FirstOrDefault(x => x.Name.Equals(requestedEvenTypeName, StringComparison.OrdinalIgnoreCase));
            if (t != null)
            {
                aEvent = (IAlexaEvent)Activator.CreateInstance(t);
            }

            return aEvent;
        }
    }
}
