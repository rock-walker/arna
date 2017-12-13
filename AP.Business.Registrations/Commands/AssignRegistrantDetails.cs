namespace AP.Business.Registration.Commands
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AP.Infrastructure.Messaging;
    using System.Collections.Generic;

    public class AssignRegistrantDetails : ICommand
    {
        public AssignRegistrantDetails()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }

        public Guid OrderId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[\w-]+(\.?[\w-])*\@[\w-]+(\.[\w-]+)+", ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        public Guid UserId { get; set; }

        public string Description { get; set; }
        public List<int> CategoryIds { get; set; }
        public DateTime? AssignedDate { get; set; }
    }
}
