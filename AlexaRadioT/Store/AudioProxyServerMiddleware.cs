using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlexaRadioT.Store
{
    //Alexa requires audio to be served via HTTPS
    //So need to proxy external http trough the current server
    //http://overengineer.net/creating-a-simple-proxy-server-middleware-in-asp-net-core
    public class AudioProxyServerMiddleware
    {
        private readonly RequestDelegate _next;

        public AudioProxyServerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var endRequest = false;
            if (context.Request.Path.Value.Equals("/proxyAudio", StringComparison.OrdinalIgnoreCase))
            {
                const string key = "url";
                if (context.Request.Query.ContainsKey(key))
                {
                    var url = context.Request.Query[key][0];
                    if (IsAllowedProxyUrlForSkill(new Uri(url)))
                    {
                        await StreamAsync(context, url);
                        endRequest = true;
                    }
                }
            }
            if (!endRequest)
            {
                await _next(context);
            }
        }

        private bool IsAllowedProxyUrlForSkill(Uri url) {
            if (ApplicationSettingsService.Skill.ProxyPodcastAudio
                && url.Host.Equals(ApplicationSettingsService.Skill.PodcastAudioUrlFormatString.Host, StringComparison.OrdinalIgnoreCase))
                return true;
            if (ApplicationSettingsService.Skill.ProxyLiveStreamAudio
                && url.Host.Equals(ApplicationSettingsService.Skill.PodcastLiveStreamUrl.Host, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private static async Task StreamAsync(HttpContext context, string url)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            var webRequest = new HttpClient(httpClientHandler);

            var buffer = new byte[4 * 1024];
            var localResponse = context.Response;
            try
            {
                using (var remoteStream = await webRequest.GetStreamAsync(url))
                {
                    var bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                    localResponse.Clear();
                    localResponse.ContentType = "audio/mpeg";

                    var fileName = Path.GetFileName(url);
                    localResponse.Headers.Add("Content-Disposition", "filename=" + fileName);

                    while (bytesRead > 0) // && localResponse.IsClientConnected)
                    {
                        await localResponse.Body.WriteAsync(buffer, 0, bytesRead);
                        bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogException(e);
            }
        }
    }

    public static class AudioProxyServerMiddlewareExtension
    {
        public static IApplicationBuilder UseAudioProxyServer(this IApplicationBuilder builder)
        {
            return builder.Use(next => new AudioProxyServerMiddleware(next).Invoke);
        }
    }
}
