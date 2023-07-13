using System;

namespace YouTubeCommon.Models
{
    public class YoutubeSearchItem
    {
        public Uri ThumbnailUrl { get; set; }
        public string VideoTitle { get; set; }
        public string VideoId { get; set; }
        public string VideoLenght { get; set; }
        public string ChannelName { get; set; }
    }
}
