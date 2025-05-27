using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pb304PetShop.Areas.Admin.Data;
using Pb304PetShop.Areas.Admin.Extensions;
using Pb304PetShop.Controllers;
using Pb304PetShop.DataContext;
using Pb304PetShop.DataContext.Entities;

namespace Pb304PetShop.Areas.Admin.Controllers
{
    public class SliderController : AdminController
    {
        private readonly AppDbContext _dbContext;

        public SliderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _dbContext.Sliders.ToListAsync();

            return View(sliders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var slider = await _dbContext.Sliders.FindAsync(id);

            return View(slider);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }

            if (!slider.ImageFile.IsImage())
            {
                ModelState.AddModelError("ImageFile", "Sekil secilmelidir!");

                return View(slider);
            }

            if (!slider.ImageFile.IsAllowedSize(1))
            {
                ModelState.AddModelError("ImageFile", "Sekil hecmi 1mb-dan cox ola bilmez");

                return View(slider);
            }

            var unicalFileName = await slider.ImageFile.GenerateFile(FilePathConstants.SliderPath);

            slider.ImageUrl = unicalFileName;

            await _dbContext.Sliders.AddAsync(slider);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]RequestModel requestModel)
        {
            var slider = await _dbContext.Sliders.FindAsync(requestModel.Id);

            if (slider == null) return NotFound();

            var removedSlider = _dbContext.Sliders.Remove(slider);
            await _dbContext.SaveChangesAsync();

            if (removedSlider != null)
            {
                System.IO.File.Delete(Path.Combine(FilePathConstants.SliderPath, slider.ImageUrl));
            }

            return Json(removedSlider.Entity);
        }

        public async Task<IActionResult> Update(int id)
        {
            var slider = await _dbContext.Sliders.FindAsync(id);

            if (slider == null) return NotFound();

            return View(slider);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }

            var existSlider = await _dbContext.Sliders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == slider.Id);

            if (existSlider == null) return NotFound();

            var previusFileName = existSlider.ImageUrl;

            if (slider.ImageFile == null)
            {
                slider.ImageUrl = existSlider.ImageUrl;
            }
            else
            {
                if (!slider.ImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile", "Sekil secilmelidir!");

                    return View(slider);
                }

                if (!slider.ImageFile.IsAllowedSize(1))
                {
                    ModelState.AddModelError("ImageFile", "Sekil hecmi 1mb-dan cox ola bilmez");

                    return View(slider);
                }

                var unicalFileName = await slider.ImageFile.GenerateFile(FilePathConstants.SliderPath);

                slider.ImageUrl = unicalFileName;
            }

            var updatedSlider = _dbContext.Sliders.Update(slider);
            await _dbContext.SaveChangesAsync();

            if (updatedSlider != null)
            {
                System.IO.File.Delete(Path.Combine(FilePathConstants.SliderPath, previusFileName));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
