public class BookDto
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Genre { get; set; }
    public string ISBN { get; set; }
    public string Publisher { get; set; }
    public DateTime? PublishDate { get; set; }
    public string Language { get; set; }
    public string Format { get; set; }
    public int? Pages { get; set; }
    public string Dimensions { get; set; }
    public string Weight { get; set; }
    public string ImageUrl { get; set; }
    public bool IsBestseller { get; set; }
    public bool IsNewRelease { get; set; }
    public bool IsOnSale { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
}