using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pb304PetShop.Areas.Admin.Data;
using Pb304PetShop.Areas.Admin.Extensions;
using Pb304PetShop.DataContext;
using Pb304PetShop.DataContext.Entities;

namespace Pb304PetShop.Areas.Admin.Controllers
{
    public class ProductController : AdminController
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _dbContext.Products
                .Include(x => x.Category)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.Images)
                .SingleOrDefaultAsync(x => x.Id == id);

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            var categoryListItems = categories.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
           
            var productCreateModel = new ProductCreateViewModel
            {
                Name = "",
                CategorySelectListItems = categoryListItems,
                CoverImageFile = null
            };

            return View(productCreateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            var categories = await _dbContext.Categories.ToListAsync();
            var categoryListItems = categories.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();

            if (!ModelState.IsValid)
            {
                model.CategorySelectListItems = categoryListItems;
                return View(model);
            }

            if (!model.CoverImageFile.IsImage())
            {
                ModelState.AddModelError("ImageFile", "Sekil secilmelidir!");
                model.CategorySelectListItems = categoryListItems;

                return View(model);
            }

            if (!model.CoverImageFile.IsAllowedSize(1))
            {
                ModelState.AddModelError("ImageFile", "Sekil hecmi 1mb-dan cox ola bilmez");
                model.CategorySelectListItems = categoryListItems;

                return View(model);
            }
            
            var productImages = new List<ProductImage>();

            bool isValidImages = true;

            foreach (var item in model.ImagesFiles ?? [])
            {
                if (!item.IsImage())
                {
                    isValidImages = false;
                    ModelState.AddModelError("", $"{item.FileName}-sekil olmalidir");
                }

                if (!item.IsAllowedSize(1))
                {
                    isValidImages = false;
                    ModelState.AddModelError("", $"{item.FileName}-hecmi 1 mb-dan cox olmamalidir");
                }
            }

            if (!isValidImages)
            {
                model.CategorySelectListItems = categoryListItems;
                return View(model);
            }

            foreach (var item in model.ImagesFiles ?? [])
            {
                var unicalFileName = await item.GenerateFile(FilePathConstants.ProductPath);
                productImages.Add(new ProductImage { Name = unicalFileName });
            }

            var unicalCoverImageFileName = await model.CoverImageFile.GenerateFile(FilePathConstants.ProductPath);

            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                CoverImageUrl = unicalCoverImageFileName,
                Images = productImages,
                CategoryId = model.CategoryId
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
