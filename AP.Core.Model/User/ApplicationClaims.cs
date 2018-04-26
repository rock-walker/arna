using AP.Core.Model.Attributes;

namespace AP.Core.Model.User
{
    public enum ApplicationClaims
    {
        [Value("None")]
        None,

        [Value("Verified")]
        Verified,

        [Value("Accomplished")]
        Accomplished,
    }
}
