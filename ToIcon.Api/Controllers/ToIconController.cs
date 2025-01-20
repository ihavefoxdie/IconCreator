using System.Data;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using ToIcon.Api.Models;

namespace ToIcon.Api.Controllers;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
[ApiController]
[Route("api/[controller]/[action]")]
public class ToIconController : ControllerBase
{
    private static IWebHostEnvironment _webHostEnv;
    private static string _lastFileName;
    public ToIconController(IWebHostEnvironment webHostEnv)
    {
        _webHostEnv = webHostEnv;
    }

    [HttpPost]
    public async Task<IActionResult> ConvertToIco([FromForm] UploadedFile file)
    {
        try
        {
            if (file.File.Length > 0 && file.File != null)
            {
                string path = _webHostEnv.ContentRootPath + "\\Uploaded Files\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (System.IO.File.Exists(path + Path.GetFileNameWithoutExtension(file.File.FileName) + ".ico"))
                {
                    System.IO.File.Delete(path + Path.GetFileNameWithoutExtension(file.File.FileName) + ".ico");
                }

                using (FileStream stream = System.IO.File.Create(path + Path.GetFileNameWithoutExtension(file.File.FileName) + ".ico"))
                {
                    using (MemoryStream memoryStream = new())
                    {
                        await file.File.CopyToAsync(memoryStream);
                        Image img = Image.FromStream(memoryStream);
                        IconHelper.ConvertToIcon(memoryStream, stream, img.Height, true);
                        await memoryStream.FlushAsync();
                        _lastFileName = path + Path.GetFileNameWithoutExtension(file.File.FileName) + ".ico";
                    }
                    await stream.FlushAsync();
                }

                return Ok();
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public IActionResult GetIcon()
    {
        try
        {
            if (System.IO.File.Exists(_lastFileName))
            {
                return PhysicalFile(_lastFileName, "image/ico");
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
    }
}
