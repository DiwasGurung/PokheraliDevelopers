public class OrderDto
{
    public string ShippingAddress { get; set; }
    public string ShippingCity { get; set; }
    public string ShippingState { get; set; }
    public string ShippingZipCode { get; set; }
    public string PaymentMethod { get; set; }
    public List<OrderItemDto> Items { get; set; }
}