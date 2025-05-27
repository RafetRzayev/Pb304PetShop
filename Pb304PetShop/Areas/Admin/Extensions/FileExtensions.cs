using Pb304PetShop.DataContext.Entities;

namespace Pb304PetShop.Areas.Admin.Extensions
{
    public static class FileExtensions
    {
        public static bool IsImage(this IFormFile formFile)
        {
            return formFile.ContentType.Contains("image");
        }

        public static bool IsAllowedSize(this IFormFile formFile, double mb)
        {
            return formFile.Length < mb * 1024 * 1024;
        }

        public async static Task<string> GenerateFile(this IFormFile formFile, string folderPath)
        {
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
           
            var unicalFileName = $"{Path.GetFileNameWithoutExtension(formFile.FileName)}-{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}";
            var filePath = Path.Combine(folderPath, unicalFileName);

            var fs = new FileStream(filePath, FileMode.Create);
            await formFile.CopyToAsync(fs);
           
            fs.Close();

            return unicalFileName;
        }
    }
}
