using Microsoft.AspNetCore.Mvc.Rendering;
using Pb304PetShop.DataContext.Entities;

namespace Pb304PetShop.Areas.Admin.Data
{
    public class ProductCreateViewModel
    {
        public required string Name { get; set; }
        public string? CoverImageUrl { get; set; }
        public required IFormFile CoverImageFile { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<SelectListItem> CategorySelectListItems { get; set; } = [];
        public IFormFile[]? ImagesFiles { get; set; }
    }

    public class ProductUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? CoverImageUrl { get; set; }
        public IFormFile? CoverImageFile { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<SelectListItem> CategorySelectListItems { get; set; } = [];
        public IFormFile[]? ImagesFiles { get; set; }
        public List<string> ImageUrls { get; set; } = [];
    }
}
