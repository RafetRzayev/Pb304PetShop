namespace Pb304PetShop.DataContext.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? DetailDescription { get; set; }
        public string? AdditionalInformation { get; set; }  
        public required string CoverImageUrl { get; set; }
        public string? Code { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public List<ProductImage> Images { get; set; } = new();
        public List<ProductColor> ProductColors { get; set; } = new();
        public List<ProductCategory> ProductCategories { get; set; } = new();
        public List<ProductTag> ProductTags { get; set; } = new();
    }

    public class Color
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<ProductColor> ProductColors { get; set; } = new();
    }

    public class ProductColor
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int ColorId { get;set; }
        public Color? Color { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<ProductCategory> ProductCategories { get; set; } = new();
    }

    public class ProductCategory
    {
        public int Id { get; set; }
        public int ProductId { get;set;}
        public Product? Product { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }

    public class ProductImage
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int ColorId { get; set; }
        public Color? Color { get; set; }
    }

    public enum Size
    {
        XS,
        S,
        M,
        L,
        XL,
        XXL
    }

    public class StockByColor
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product{ get; set; }
        public int ColorId { get; set; }
        public Color? Color{ get; set; }
        public int Quantity { get; set; }
    }

    public class StockBySize
    {
        public int Id { get; set; }
        public int StockByColorId { get; set; }
        public StockByColor? StockByColor { get; set; }
        public Size Size { get; set; }
        public int Quantity { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<ProductTag> ProductTags { get; set; } = new();
    }

    public class ProductTag
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
