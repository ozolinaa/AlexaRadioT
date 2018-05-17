using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlexaRadioTDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            //2018-05-12 17:21:28.3013991
            DateTime dateStart = new DateTime(2018, 5, 12);
            DateTime dateEnd = dateStart.AddDays(1);
            string debugApi = "http://localhost:49968/api/alexa";

            var requests = Log.AlexaRequestSelect(Int32.MaxValue).Where(x => x.LoggedDateTime > dateStart && x.LoggedDateTime < dateEnd).OrderBy(x=>x.LoggedDateTime);

            using (HttpClient httpClient = new HttpClient())
            {
                foreach (RequestLogItem request in requests)
                {
                    var r = httpClient.PostAsync(debugApi, new StringContent(request.Text)).Result;
                    //Wait 0.5 sexont untill next request
                    Task.Delay(500).Wait();
                }
            }

            Console.ReadLine();
        }
    }
}
