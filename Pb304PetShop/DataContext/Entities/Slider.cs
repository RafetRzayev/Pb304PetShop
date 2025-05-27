using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pb304PetShop.DataContext.Entities
{
    public class Slider
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        [NotMapped]
        public required IFormFile ImageFile { get; set; }
    }
}
