using AP.Core.Model.Attributes;

namespace AP.Core.Model.User
{
    public enum Roles
    {
        [Value("Administrator")]
        Administrator,

        [Value("PowerUser")]
        PowerUser,

        [Value("Moderator")]
        Moderator,

        [Value("Client")]
        Client,

        [Value("Master")]
        Master
    }
}
