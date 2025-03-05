using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Image.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost(Name = "Upload")]
        public ActionResult<UploadDto> Upload(IFormFile file)
        {
            #region ValidationExtensions
            var extension = Path.GetExtension(file.FileName);

            //TODO: extension validation in appsettings.json file this best practice

            var extensions = new List<string> { ".jpg", ".jpeg", ".png", ".svg" };

            var isAllowedExtension =extensions.Contains(extension, StringComparer.OrdinalIgnoreCase);

            if (!isAllowedExtension)
            {
                return BadRequest(new UploadDto {
                    IsSuccess = false,
                    Message = "Invalid file extension"
                });
            }
            #endregion

            #region ValidationSize
            var isSizeValid = file.Length is > 0 and < 4_000_000;
            if (!isSizeValid)
            {
                return BadRequest(new UploadDto
                {
                    IsSuccess = false,
                    Message = "Size is not allowed"
                });
            }
            #endregion

            #region StoreFile
            var fileName = $"{Guid.NewGuid()}{extension}";
            var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            var path = Path.Combine(imagesPath, fileName);

            using var stream = new FileStream(path, FileMode.Create);

            file.CopyTo(stream);
            #endregion

            #region GenerateUrl
            var upload = new UploadDto
            {
                IsSuccess = true,
                Url = $"{Request.Scheme}://{Request.Host}/Images/{fileName}"
            };

            return Ok(upload);
            #endregion

        }
    }
}
