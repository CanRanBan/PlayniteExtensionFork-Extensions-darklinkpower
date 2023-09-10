using Playnite.SDK.Data;

namespace JastUsaLibrary.Models
{
    public partial class AuthenticationRefreshResponse
    {
        [SerializationPropertyName("token")]
        public string Token { get; set; }

        [SerializationPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}