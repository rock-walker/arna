﻿namespace AP.EntityModel.Booking
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Infrastructure.Utils;

    public class AnchorType
    {
        public AnchorType()
        {
            ID = GuidUtil.NewSequentialId();
        }

        public Guid ID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(70, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(250)]
        public string Description { get; set; }

        [Range(0, 100000)]
        public int Quantity { get; set; }

        [Range(0, 50000)]
        public decimal Price { get; set; }
    }
}