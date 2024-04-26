using AzureStorageLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using MVCWebApp.Models;

namespace MVCWebApp.Controllers
{
    public class BlobsController : Controller
    {
        private readonly IBlobStorage _blobStorage;

        public BlobsController(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {
            var names = _blobStorage.GetNames(EContainerName.pdfs);
            string blobUrl = $"{_blobStorage.BlobUrl}/{EContainerName.pdfs.ToString()}";

            ViewBag.logs = await _blobStorage.GetLogsAsync("controller.txt");
            ViewBag.blobs = names.Select(x => new FileBlob { Name = x, Url = $"{blobUrl}/{x}" }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            try
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
                if (picture.FileName.Contains("pdf"))
                {
                    await _blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pdfs);
                    await _blobStorage.SetLogAsync("Pdf uploaded successfully", "controller.txt");
                }
                else
                {
                    await _blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);
                    await _blobStorage.SetLogAsync("Image uploaded successfully", "controller.txt");
                }
            }
            catch (Exception)
            {
                await _blobStorage.SetLogAsync("Error in File Upload", "controller.txt");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Download(string filename)
        {
            try
            {

                var stream = await _blobStorage.DownloadAsync(filename, EContainerName.pdfs);
                await _blobStorage.SetLogAsync("File downloaded successfully", "controller.txt");
                return File(stream, "application/octet-stream", filename);
            }
            catch (Exception)
            {
                await _blobStorage.SetLogAsync("Error in File Download", "controller.txt");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string filename)
        {
            await _blobStorage.DeleteAsync(filename, EContainerName.pdfs);
            return RedirectToAction("Index");
        }
    }
}
