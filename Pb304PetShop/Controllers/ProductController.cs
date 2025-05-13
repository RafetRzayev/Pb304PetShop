using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pb304PetShop.DataContext;

namespace Pb304PetShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _dbContext.Products
                .Include(x => x.Images)
                .Include(x => x.ProductColors).ThenInclude(x => x.Color)
                .Include(x => x.ProductCategories).ThenInclude(x => x.Category)
                .Include(x => x.ProductTags).ThenInclude(x => x.Tag)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
