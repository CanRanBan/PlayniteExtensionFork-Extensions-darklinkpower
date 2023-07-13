using Playnite.SDK.Data;

namespace JastUsaLibrary.Models
{
    public class GenerateLinkResponse
    {
        [SerializationPropertyName("url")]
        public string Url { get; set; }
    }
}