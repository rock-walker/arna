using AP.EntityModel.Common;
using AP.ViewModel.Common;

namespace AP.EntityModel.Mappers
{
    public static class ContactsMapper
    {
        public static ContactViewModel MapTo(this Contact data)
        {
            return new ContactViewModel
            {
                Chat = data.Chat,
                Email = data.Email,
                Mobile = data.Mobile,
                Municipal = data.Municipal,
                Web = data.Web
            };
        }
    }
}
