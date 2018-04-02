namespace AP.Core.Model.User
{
    public class JwtResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public IdentityStatus Status { get; set; }
        public string Message { get; set; }
    }
}
