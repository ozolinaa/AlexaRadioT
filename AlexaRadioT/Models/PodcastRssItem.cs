﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace AlexaRadioT.Models
{
    public class PodcastRssItem
    {
        public string Title { get; set; }
        public int EpisodeNumber { get; set; }
        public DateTime PublishedUtc { get; set; }
        public Uri PageUrl { get; set; }
        public Uri AudioUrl { get; set; }

        public PodcastRssItem()
        {
        }
        public PodcastRssItem(XmlNode rssNode)
        {
            Title = rssNode.SelectSingleNode("title").InnerText;
            EpisodeNumber = Convert.ToInt32(Regex.Replace(Title, "[^0-9]", ""));
            PublishedUtc = _parseDateTimeUTCFromPubDate(rssNode.SelectSingleNode("pubDate").InnerText);
            PageUrl = new Uri(rssNode.SelectSingleNode("link").InnerText);
            AudioUrl = new Uri(rssNode.SelectSingleNode("enclosure[@type='audio/mp3']").Attributes["url"].Value);
        }

        private static DateTime _parseDateTimeUTCFromPubDate(string pubDateInnerText)
        {
            Dictionary<string, string> _timeZones = new Dictionary<string, string>() {
                {"ACDT", "+1030"},
                {"ACST", "+0930"},
                {"ADT", "-0300"},
                {"AEDT", "+1100"},
                {"AEST", "+1000"},
                {"AHDT", "-0900"},
                {"AHST", "-1000"},
                {"AST", "-0400"},
                {"AT", "-0200"},
                {"AWDT", "+0900"},
                {"AWST", "+0800"},
                {"BAT", "+0300"},
                {"BDST", "+0200"},
                {"BET", "-1100"},
                {"BST", "-0300"},
                {"BT", "+0300"},
                {"BZT2", "-0300"},
                {"CADT", "+1030"},
                {"CAST", "+0930"},
                {"CAT", "-1000"},
                {"CCT", "+0800"},
                {"CDT", "-0500"},
                {"CED", "+0200"},
                {"CET", "+0100"},
                {"CEST", "+0200"},
                {"CST", "-0600"},
                {"EAST", "+1000"},
                {"EDT", "-0400"},
                {"EED", "+0300"},
                {"EET", "+0200"},
                {"EEST", "+0300"},
                {"EST", "-0500"},
                {"FST", "+0200"},
                {"FWT", "+0100"},
                {"GMT", "GMT"},
                {"GST", "+1000"},
                {"HDT", "-0900"},
                {"HST", "-1000"},
                {"IDLE", "+1200"},
                {"IDLW", "-1200"},
                {"IST", "+0530"},
                {"IT", "+0330"},
                {"JST", "+0900"},
                {"JT", "+0700"},
                {"MDT", "-0600"},
                {"MED", "+0200"},
                {"MET", "+0100"},
                {"MEST", "+0200"},
                {"MEWT", "+0100"},
                {"MST", "-0700"},
                {"MT", "+0800"},
                {"NDT", "-0230"},
                {"NFT", "-0330"},
                {"NT", "-1100"},
                {"NST", "+0630"},
                {"NZ", "+1100"},
                {"NZST", "+1200"},
                {"NZDT", "+1300"},
                {"NZT", "+1200"},
                {"PDT", "-0700"},
                {"PST", "-0800"},
                {"ROK", "+0900"},
                {"SAD", "+1000"},
                {"SAST", "+0900"},
                {"SAT", "+0900"},
                {"SDT", "+1000"},
                {"SST", "+0200"},
                {"SWT", "+0100"},
                {"USZ3", "+0400"},
                {"USZ4", "+0500"},
                {"USZ5", "+0600"},
                {"USZ6", "+0700"},
                {"UT", "-0000"},
                {"UTC", "-0000"},
                {"UZ10", "+1100"},
                {"WAT", "-0100"},
                {"WET", "-0000"},
                {"WST", "+0800"},
                {"YDT", "-0800"},
                {"YST", "-0900"},
                {"ZP4", "+0400"},
                {"ZP5", "+0500"},
                {"ZP6", "+0600"}
            };
            string[] dateParts = pubDateInnerText.Split(' ');
            dateParts[dateParts.Length - 1] = _timeZones[dateParts[dateParts.Length - 1]];
            return DateTime.Parse(string.Join(" ", dateParts)).ToUniversalTime();
        }
    }
}
