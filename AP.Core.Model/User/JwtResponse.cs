using AP.Core.Model.Api;

namespace AP.Core.Model.User
{
    public class JwtResponse : ApiResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}
