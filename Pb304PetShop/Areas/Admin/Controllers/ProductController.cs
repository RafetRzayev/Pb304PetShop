using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pb304PetShop.Areas.Admin.Data;
using Pb304PetShop.Areas.Admin.Extensions;
using Pb304PetShop.Controllers;
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
                .Include(x => x.ProductTags).ThenInclude(x => x.Tag)
                .SingleOrDefaultAsync(x => x.Id == id);

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            var categoryListItems = categories.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();

            var tags = await _dbContext.Tags.ToListAsync();
            var tagListItems = tags.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();

            var productCreateModel = new ProductCreateViewModel
            {
                Name = "",
                CategorySelectListItems = categoryListItems,
                TagSelectListItems = tagListItems,
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

            var tags = await _dbContext.Tags.ToListAsync();
            var tagListItems = tags.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();

            if (!ModelState.IsValid)
            {
                model.CategorySelectListItems = categoryListItems;
                model.TagSelectListItems = tagListItems;

                return View(model);
            }

            if (!model.CoverImageFile.IsImage())
            {
                ModelState.AddModelError("ImageFile", "Sekil secilmelidir!");
                model.CategorySelectListItems = categoryListItems;
                model.TagSelectListItems = tagListItems;

                return View(model);
            }

            if (!model.CoverImageFile.IsAllowedSize(1))
            {
                ModelState.AddModelError("ImageFile", "Sekil hecmi 1mb-dan cox ola bilmez");
                model.CategorySelectListItems = categoryListItems;
                model.TagSelectListItems = tagListItems;

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
                model.TagSelectListItems = tagListItems;

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
                CategoryId = model.CategoryId,
                ProductTags = model.TagIdList.Select(x => new ProductTag { TagId = x }).ToList()
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] RequestModel requestModel)
        {
            var product = await _dbContext.Products
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == requestModel.Id);

            if (product == null) return NotFound();

            var removedProduct = _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            if (removedProduct != null)
            {
                System.IO.File.Delete(Path.Combine(FilePathConstants.ProductPath, product.CoverImageUrl));

                foreach (var item in product.Images)
                {
                    System.IO.File.Delete(Path.Combine(FilePathConstants.ProductPath, item.Name));
                }
            }

            return Json(removedProduct.Entity);
        }

        public async Task<IActionResult> Update(int id)
        {
            var product = await _dbContext.Products
                .Include(x => x.Images)
                .Include(x => x.ProductTags).ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x => x.Id == id);

            var categories = await _dbContext.Categories.ToListAsync();
            var categoryListItems = categories.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();

            var tags = await _dbContext.Tags.ToListAsync();
            List<SelectListItem> tagListItems = [];

            foreach (var item in tags)
            {
                if (product.ProductTags.Find(x => x.TagId == item.Id) != null)
                    continue;

                tagListItems.Add(new SelectListItem(item.Name, item.Id.ToString()));
            }

            if (product == null) return NotFound();

            var updateViewModel = new ProductUpdateViewModel
            {
                Name = product.Name,
                CoverImageUrl = product.CoverImageUrl,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategorySelectListItems = categoryListItems,
                ImageUrls = product.Images.Select(x => x.Name).ToList(),
                ProductTags = product.ProductTags,
                TagSelectListItems = tagListItems,
            };

            return View(updateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductUpdateViewModel model)
        {
            var product = await _dbContext.Products.Include(x => x.ProductTags).FirstOrDefaultAsync(x => x.Id == model.Id);

            if (product == null) return NotFound();

            var categories = await _dbContext.Categories.ToListAsync();
            var categoryListItems = categories.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();

            var tags = await _dbContext.Tags.ToListAsync();
            List<SelectListItem> tagListItems = [];

            foreach (var item in tags)
            {
                if (product.ProductTags.Find(x => x.TagId == item.Id) != null)
                    continue;

                tagListItems.Add(new SelectListItem(item.Name, item.Id.ToString()));
            }

            if (!ModelState.IsValid)
            {
                model.CategorySelectListItems = categoryListItems;
                model.TagSelectListItems = tagListItems;
                return View(model);
            }

            product.Name = model.Name;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;

            foreach (var item in model.TagIdList)
            {
                if (product.ProductTags.Find(x => x.TagId == item) != null) continue;
                
                product.ProductTags.Add(new ProductTag { TagId = item });
            }

            if (model.CoverImageFile != null)
            {
                if (!model.CoverImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile", "Sekil secilmelidir!");
                    model.CategorySelectListItems = categoryListItems;
                    model.TagSelectListItems = tagListItems;

                    return View(model);
                }

                if (!model.CoverImageFile.IsAllowedSize(1))
                {
                    ModelState.AddModelError("ImageFile", "Sekil hecmi 1mb-dan cox ola bilmez");
                    model.CategorySelectListItems = categoryListItems;
                    model.TagSelectListItems = tagListItems;

                    return View(model);
                }

                var unicalCoverImageFileName = await model.CoverImageFile.GenerateFile(FilePathConstants.ProductPath);

                if (product.CoverImageUrl != null)
                {
                    System.IO.File.Delete(Path.Combine(FilePathConstants.ProductPath, product.CoverImageUrl));
                }
                       
                product.CoverImageUrl = unicalCoverImageFileName;
            }

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
                model.TagSelectListItems = tagListItems;

                return View(model);
            }

            foreach (var item in model.ImagesFiles ?? [])
            {
                var unicalFileName = await item.GenerateFile(FilePathConstants.ProductPath);
                product.Images.Add(new ProductImage { Name = unicalFileName });
            }

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveImage([FromBody] RequestModel requestModel)
        {
            if (string.IsNullOrEmpty(requestModel.ImageName)) return BadRequest();

            var productImage = await _dbContext.ProductImages.FirstOrDefaultAsync(x => x.Name == requestModel.ImageName);

            if (productImage == null) return BadRequest();

            var removedImage = _dbContext.ProductImages.Remove(productImage);
            await _dbContext.SaveChangesAsync();

            if (removedImage != null)
            {
                System.IO.File.Delete(Path.Combine(FilePathConstants.ProductPath, requestModel.ImageName));
            }

            return Json(removedImage.Entity);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTag([FromBody]RequestModel requestModel)
        {
            var productTag = await _dbContext.ProductTags
                .Include(x => x.Tag)
                .FirstOrDefaultAsync(x=>x.Id == requestModel.Id);

            if (productTag == null) return BadRequest();

            var removedTag = productTag.Tag;

            _dbContext.ProductTags.Remove(productTag);

            await _dbContext.SaveChangesAsync();

            return Json(new {removedTag.Id, removedTag.Name});
        }
    }
}
