using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlexaRadioT.Models;
using AlexaRadioT.Store;
using System.Net.Http;
using System.IO;

namespace AlexaRadioT.Controllers
{
    [Route("PlayList")]
    public class PlayListController : Controller
    {
        [HttpGet]
        [Route("LiveStream")]
        public FileResult LiveStream()
        {
            return GeneratePlayListForAudioUrl(ApplicationSettingsService.Skill.LiveStreamUrl);
        }

        [HttpGet]
        [Route("Podcast/{podcastNumber}")]
        public FileResult Podcast(int podcastNumber)
        {
            PodcastEnity podcast = RadioT.GetPodcastDetails(podcastNumber);
            return GeneratePlayListForAudioUrl(new Uri(podcast.AudioURL));
        }

        [HttpGet]
        [Route("Podcast/{podcastNumber}/proxy")]
        public async Task<FileResult> PodcastAsync(int podcastNumber)
        {
            PodcastEnity podcast = RadioT.GetPodcastDetails(podcastNumber);
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };
            var client = new HttpClient(httpClientHandler);

            Uri fileUri = new Uri(podcast.AudioURL);
            string fileExtension = Path.GetExtension(fileUri.Segments.Last());
            string fileName = Path.GetFileName(fileUri.Segments.Last()) + (string.IsNullOrEmpty(fileExtension) ? ".mp3" : fileExtension);

            using (var remoteStream = await client.GetStreamAsync(podcast.AudioURL))
            {
                return File(remoteStream, "audio/mpeg", fileName);
            }
        }

        private FileResult GeneratePlayListForAudioUrl(Uri url) {
            byte[] fileBytes = System.Text.Encoding.ASCII.GetBytes("#EXTM3U\n"+ url);
            return File(fileBytes, "application/x-mpegurl", System.IO.Path.GetFileName(url.Segments.Last()) + ".m3u");
        }
    }
}