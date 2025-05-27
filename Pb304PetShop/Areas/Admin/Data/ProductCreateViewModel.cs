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
        public List<string> Images { get; set; } = new();
        public IFormFile[]? ImagesFiles { get; set; }
    }
}
