namespace Pb304PetShop.DataContext.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<ProductTag> ProductTags { get; set; } = [];
    }

    public class ProductTag
    {
        public int Id { get; set; }
        public int TagId {  get; set; }
        public Tag? Tag { get; set; }
        public int ProductId { get;set; }
        public Product? Product { get; set; }
    }
}
