using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlexaRadioT.Models;
using AlexaRadioT.Store;

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

        private FileResult GeneratePlayListForAudioUrl(Uri url) {
            byte[] fileBytes = System.Text.Encoding.ASCII.GetBytes("#EXTM3U\n"+ url);
            return File(fileBytes, "application/x-mpegurl", System.IO.Path.GetFileName(url.Segments.Last()) + ".m3u");
        }
    }
}