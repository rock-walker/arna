using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Common
{
	public class Gallery
	{
		public int Id { get; set; }
		public string Thumbnail { get; set; }
		public Guid FolderId { get; set; }
	}
}
