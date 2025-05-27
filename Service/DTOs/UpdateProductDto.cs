namespace Service.DTOs
{
    public class UpdateProductDto
    {
        // ID will be passed as a separate parameter to the service's update method
        public string Title { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}