using Playnite.SDK.Data;
using System;

namespace JastUsaLibrary.Models
{
    public class GenerateLinkResponse
    {
        [SerializationPropertyName("url")]
        public Uri Url { get; set; }
    }
}