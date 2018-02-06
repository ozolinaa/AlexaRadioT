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

            string host = url.Host.ToLower();

            if (ApplicationSettingsService.Skill.ProxyPodcastAudio
                && ApplicationSettingsService.Skill.AllowedDomainsForPodcastAudioProxy.Contains(host))
                return true;
            if (ApplicationSettingsService.Skill.ProxyLiveStreamAudio
                && ApplicationSettingsService.Skill.AllowedDomainsForPodcastLiveStreamProxy.Contains(host))
                return true;

            return false;
        }

        private static async Task StreamAsync(HttpContext context, string url)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            var client = new HttpClient(httpClientHandler);

            // larger buffer causes problems with live stream
            byte[] buffer = new byte[1024]; 
            var localResponse = context.Response;
            try
            {
                using (var remoteStream = await client.GetStreamAsync(url))
                {
                    localResponse.Clear();

                    localResponse.ContentType = "audio/mpeg";
                    if (ApplicationSettingsService.Skill.PodcastLiveStreamUrl == new Uri(url))
                    {
                        localResponse.Headers.Add("Cache-Control", "no-cache");
                    }
                    else
                    {
                        localResponse.Headers.Add("Cache-Control", "max-age=31536000");
                        localResponse.Headers.Add("Connection", "keep-alive");
                        localResponse.Headers.Add("Keep-Alive", "timeout=60");
                    }

                    var bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                    var fileName = Path.GetFileName(url);
                    localResponse.Headers.Add("Content-Disposition", "filename=" + fileName);

                    while (bytesRead > 0)
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
