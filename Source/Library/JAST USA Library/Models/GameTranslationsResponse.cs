using Playnite.SDK.Data;
using System.Collections.Generic;

namespace JastUsaLibrary.Models
{
    public class GameTranslationsResponse
    {
        [SerializationPropertyName("@context")]
        public string ContextApiEndpoint { get; set; }

        [SerializationPropertyName("@type")]
        public string TypeApiEndpoint { get; set; }

        [SerializationPropertyName("@id")]
        public string IdApiEndpoint { get; set; }

        [SerializationPropertyName("gamePathLinks")]
        public List<GameLink> GamePathLinks { get; set; }

        [SerializationPropertyName("gameExtraLinks")]
        public List<GameLink> GameExtraLinks { get; set; }

        [SerializationPropertyName("gamePatchLinks")]
        public List<GameLink> GamePatchLinks { get; set; }
    }

    public partial class GameLink
    {
        [SerializationPropertyName("@type")]
        public string Type { get; set; }

        [SerializationPropertyName("gameId")]
        public int GameId { get; set; }

        [SerializationPropertyName("gameLinkId")]
        public int GameLinkId { get; set; }

        [SerializationPropertyName("label")]
        public string Label { get; set; }

        [SerializationPropertyName("platforms")]
        public JastPlatform[] Platforms { get; set; }

        [SerializationPropertyName("version")]
        public string Version { get; set; }
    }
}