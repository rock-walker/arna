using AP.Core.Model.Attributes;

namespace AP.Core.Model.User
{
    public enum ApplicationClaims
    {
        [Value("Verified")]
        Verified,

        [Value("Accomplished")]
        Accomplished,
    }
}
