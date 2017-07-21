using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Common
{
	public class DomainModels
	{
		public abstract class Specialist
		{
			[Key]
			[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
			public int Id { get; set; }
			[StringLength(40)]
			public string First { get; set; }
			[StringLength(40)]
			public string Last { get; set; }
			[StringLength(40)]
			public string Middle { get; set; }
			public virtual AvatarImage Ava { get; set; }
			public virtual Category Category { get; set; }
		}

		public class AvatarImage
		{
			public Guid ID { get; set; }
			public virtual byte[] Image { get; set; }
		}

		public class BaseCategory
		{
			public int Id { get; set; }
			public int Parent { get; set; }
		}
	}
}
