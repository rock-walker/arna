using System;

namespace AP.Shared.Geo.Models
{
    public class GooglePlacesReview
    {
        public DateTime ReviewDate { get; set; }
        public string ReviewText { get; set; }
        public string AuthorName { get; set; }
        public decimal Rating { get; set; }
        public string Language { get; set; }
    }
}
