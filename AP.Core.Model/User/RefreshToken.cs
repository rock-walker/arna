using System;

namespace AP.Core.Model.User
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public bool Revoked { get; set; }
    }
}
