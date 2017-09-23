using AP.EntityModel.Common;
using AP.ViewModel.Common;

namespace AP.EntityModel.Mappers
{
    public static class ContactsMapper
    {
        public static ContactViewModel MapTo(this ContactData data)
        {
            return new ContactViewModel
            {
                Email = data.Email,
                Mobile = data.Mobile,
                Web = data.Web
            };
        }
    }
}
