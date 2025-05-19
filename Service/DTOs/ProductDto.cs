namespace Service.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int UnitPrice { get; set; } // Or decimal if you changed it
        public int Quantity { get; set; }
    }
}