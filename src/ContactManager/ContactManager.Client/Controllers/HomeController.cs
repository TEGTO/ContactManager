using AutoMapper;
using ContactManager.Client.Models;
using ContactManager.Client.Services;
using ContractManager.Communication.Dtos.Endpoints.Update;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ContactManager.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactService contactService;
        private readonly IFileValidationService fileValidationService;
        private readonly IMapper mapper;

        public HomeController(IContactService contactService, IFileValidationService fileValidationService, IMapper mapper)
        {
            this.contactService = contactService;
            this.fileValidationService = fileValidationService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var contacts = await contactService.GetAllAsync(cancellationToken);
            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile file, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction("Index");
            }

            if (!fileValidationService.ValidateCsvFile(file, out var errorMessage))
            {
                TempData["Error"] = errorMessage;
                return RedirectToAction("Index");
            }

            var result = await contactService.UploadFileToApiAsync(file, cancellationToken);

            if (result.IsSuccessStatusCode)
            {
                TempData["Success"] = "File uploaded successfully.";
            }
            else
            {
                TempData["Error"] = $"Error uploading file: {result.ReasonPhrase}";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateContact(string id, CancellationToken cancellationToken)
        {
            var response = await contactService.GetByIdAsync(id, cancellationToken);

            if (response == null)
            {
                return RedirectToAction("Index");
            }

            var request = mapper.Map<UpdateRequest>(response);

            return View("UpdateContact", request);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateContactForm(UpdateRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdateContact", request);
            }

            var result = await contactService.UpdateAsync(request, cancellationToken);

            if (!result.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Error updating: {result.ReasonPhrase}";
            }

            return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteById(string id, CancellationToken cancellationToken)
        {
            var result = await contactService.DeleteByIdAsync(id, cancellationToken);

            if (!result.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Error deleting: {result.ReasonPhrase}";
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
