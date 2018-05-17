using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AlexaRadioT.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Debug()
        {
            return View();
        }

        public IActionResult DebugLog()
        {
            IEnumerable<Models.LogItem> debugLog = Store.Log.DebugSelect(50);
            return View("Log", debugLog);
        }

        public IActionResult ExceptionLog()
        {
            IEnumerable<Models.LogItem> exceptionLog = Store.Log.ExceptionSelect();
            return View("Log", exceptionLog);
        }

        public IActionResult RequestLog()
        {
            IEnumerable<Models.RequestLogItem> requestLog = Store.Log.AlexaRequestSelect();
            return View("RequestLog", requestLog);
        }

    }
}