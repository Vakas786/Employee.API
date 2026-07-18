using Microsoft.AspNetCore.Http;

namespace Employee.API.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}