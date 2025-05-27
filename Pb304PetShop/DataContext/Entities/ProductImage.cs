namespace Pb304PetShop.DataContext.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
