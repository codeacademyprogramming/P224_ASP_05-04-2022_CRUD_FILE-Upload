using System;

namespace Pustok.ViewModels.Basket
{
    public class BasketVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string MainImage { get; set; }
        public string HoverImage { get; set; }
        public double Price { get; set; }
        public Nullable<double> DiscountPrice { get; set; }
        public bool IsFeature { get; set; }
        public bool IsArrival { get; set; }
        public bool IsMostView { get; set; }
        //[Required]
        public Nullable<int> AuthorId { get; set; }
        //[Required]
        public Nullable<int> GenreId { get; set; }
        public string GenreName { get; set; }
        public int Count { get; set; }
    }
}
