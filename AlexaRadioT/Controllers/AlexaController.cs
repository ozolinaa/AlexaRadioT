using System;
using Microsoft.AspNetCore.Mvc;
using AlexaRadioT.Store;
using System.IO;
using AlexaRadioT.Models;
using Newtonsoft.Json;
using AlexaRadioT.Intents;
using AlexaRadioT.Events;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;

namespace AlexaRadioT.Controllers
{
    [Route("api/[controller]")]
    public class AlexaController : Controller
    {
        [HttpPost]
        public IActionResult IntentActionAsync()
        {
            AlexaResponse response = null;

            try
            {
                string requestBodyStr = null;
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    requestBodyStr = reader.ReadToEndAsync().Result;
                }

                Guid loggedRequestId = Log.LogAlexaRequest(requestBodyStr);
                AlexaRequest request = JsonConvert.DeserializeObject<AlexaRequest>(requestBodyStr);

#if (RELEASE)
                if(_isValidAmazonRequest(requestBodyStr, request) == false)
                    return BadRequest();
#endif

                IAlexaIntent intent = IntentFactory.GetIntentForRequest(request);
                if (intent != null)
                {
                    response = intent.ProcessRequest(request);
                }

                IAlexaEvent aEvent = EventFabric.GetEventForRequest(request);
                if (aEvent != null)
                {
                    AlexaResponse eventResponse = aEvent.ProcessRequest(request);
                    if (response == null)
                    {
                        response = eventResponse;
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

            return Ok(response);
        }


        private bool _isValidAmazonRequest(string requestBodyStr, AlexaRequest request)
        {
            if (!_validateAmazonCertificate(out X509Certificate2 cert))
                return false;
            if (!_isRequestBodySignatureValid(requestBodyStr, cert))
                return false;
            if (!_isRequestHasValidTimeStamp(request))
                return false;
            if (!_isValidApplicationId(request))
                return false;

            return true;
        }

        private static bool _isValidApplicationId(AlexaRequest request) {
            return Array.IndexOf(ApplicationSettingsService.Skill.RespondToSkillId,
                request.Context.System.Application.ApplicationId) > -1;
        }

        private bool _validateAmazonCertificate(out X509Certificate2 cert)
        {
            cert = null;

            if (!Request.Headers.ContainsKey("SignatureCertChainUrl"))
                return false;

            string signatureCertChainUrl = Request.Headers["SignatureCertChainUrl"].First().Replace("/../", "/");

            if (string.IsNullOrWhiteSpace(signatureCertChainUrl))
                return false;

            Uri certUrl = new Uri(signatureCertChainUrl);

            if (!((certUrl.Port == 443 || certUrl.IsDefaultPort)
                && certUrl.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                && certUrl.Host.Equals("s3.amazonaws.com", StringComparison.OrdinalIgnoreCase)
                && certUrl.AbsolutePath.StartsWith("/echo.api/")))
                return false;

            using (var web = new System.Net.WebClient())
            {
                byte[] certificateBytes = web.DownloadData(certUrl);
                cert = new X509Certificate2(certificateBytes);

                if (!((DateTime.TryParse(cert.GetExpirationDateString(), out DateTime expityDate)
                    && expityDate > DateTime.UtcNow)
                    && (DateTime.TryParse(cert.GetEffectiveDateString(), out DateTime effectiveDate)
                    && effectiveDate < DateTime.UtcNow)))
                    return false;

                if (!cert.Subject.Contains("CN=echo-api.amazon.com"))
                    return false;
            }

            return true;
        }

        private bool _isRequestBodySignatureValid(string requestBodyStr, X509Certificate2 cert)
        {
            if (string.IsNullOrEmpty(requestBodyStr))
                return false;

            if (!Request.Headers.ContainsKey("Signature"))
                return false;

            string signatureString = Request.Headers["Signature"].First();

            if (string.IsNullOrEmpty(signatureString))
                return false; 

            byte[] signature = Convert.FromBase64String(signatureString);

            byte[] requestSHA1Hash = null;
            using (var sha1 = SHA1.Create())
            {
                requestSHA1Hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(requestBodyStr));
            }

            using (RSA rsa = cert.GetRSAPublicKey())
            {
                if (rsa == null || rsa.VerifyHash(requestSHA1Hash, signature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1) == false)
                    return false;
            }

            return true;
        }

        private static bool _isRequestHasValidTimeStamp(AlexaRequest alexaRequest)
        {
            double maxRequestDelaySeconds = 150;
            double requestDelaySeconds = (DateTime.UtcNow - alexaRequest.Request.Timestamp).TotalSeconds;
            if (requestDelaySeconds <= 0 || requestDelaySeconds > maxRequestDelaySeconds)
                return false;

            return true;
        }

    }
}
