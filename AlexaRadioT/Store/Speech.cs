using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Store
{
    public static class Speech
    {
        public static string ToString(TimeSpan t, bool applySSML = false)
        {
            string result = "";

            if (t.Days > 0)
            {
                string ssmlFormatString = applySSML ? "<p>{0}</p>" : "{0}";
                result += string.Format(ssmlFormatString, string.Format("{0} {1} ", t.Days, t.Days > 1 ? "days" : "day"));
            }
            if (t.Hours > 0)
            {
                string ssmlFormatString = applySSML ? "<p>{0}</p>" : "{0}";
                result += string.Format(ssmlFormatString, string.Format("{0} {1} {2} ",
                    t.Days == 0 ? "" : "and",
                    t.Hours,
                    t.Hours > 1 ? "hours" : "hour"));
                if (t.Days > 0)
                    return result.TrimEnd();
            }
            if (t.Minutes > 0)
            {
                string ssmlFormatString = applySSML ? "<p>{0}</p>" : "{0}";
                result += string.Format(ssmlFormatString, string.Format("{0} {1} {2} ",
                    t.Hours == 0 ? "" : "and",
                    t.Minutes,
                    t.Minutes > 1 ? "minutes" : "minutes"));
                if (t.Hours > 0)
                    return result.TrimEnd();
            }
            if (t.Seconds > 0)
            {
                string ssmlFormatString = applySSML ? "<break strength=\"strong\"/>{0}" : "{0}";
                result += string.Format(ssmlFormatString, string.Format("{0} {1} {2} ", 
                    t.Hours == 0 && t.Minutes == 0 ? "" : "and", 
                    t.Seconds, 
                    t.Seconds > 1 ? "seconds" : "second"));
            }
            return result.TrimEnd();
        }
    }
}
