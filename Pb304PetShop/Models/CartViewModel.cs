namespace Pb304PetShop.Models
{
    public class CartViewModel
    {
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new();
    }

    public class CartItemViewModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
