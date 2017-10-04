using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Common
{
    public class ContactData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        [StringLength(128)]
        public string Mobile { get; set; } //divided by ';'
        [StringLength(256)]
        public string Web { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
    }
}
