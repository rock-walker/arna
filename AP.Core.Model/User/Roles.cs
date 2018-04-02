using AP.Core.Model.Attributes;

namespace AP.Core.Model.User
{
    public enum Roles
    {
        Invalid,

        [Value("Client")]
        Client,

        [Value("Master")]
        Master,

        [Value("Verified")]
        Verified,

        [Value("Administrator")]
        Administrator = 2147159114,

        [Value("PowerUser")]
        PowerUser = 214713911,

        [Value("Moderator")]
        Moderator = 21479114
    }
}
