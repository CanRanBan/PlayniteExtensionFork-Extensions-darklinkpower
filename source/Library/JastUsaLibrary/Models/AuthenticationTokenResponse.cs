using Playnite.SDK.Data;

namespace JastUsaLibrary.Models
{
    public class AuthenticationToken
    {
        [SerializationPropertyName("token")]
        public string Token { get; set; }

        [SerializationPropertyName("customer")]
        public string Customer { get; set; }

        [SerializationPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}