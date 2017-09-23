using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Common
{
    public class ContactData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public string Mobile { get; set; } //divided by ';'
        public string Email { get; set; }
        public string Web { get; set; }
    }
}
