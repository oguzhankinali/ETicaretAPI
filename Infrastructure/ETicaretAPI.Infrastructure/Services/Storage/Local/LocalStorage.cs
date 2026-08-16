using ETicaretAPI.Application.Abstraction.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage.Local
{
    public class LocalStorage : ILocalStorage
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocalStorage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public Task DeleteAsync(string pathOrContainerName, string fileName)
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, pathOrContainerName, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);

             return Task.CompletedTask;


        }

        public List<string> GetFiles(string pathOrContainerName)
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, pathOrContainerName);
            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            List<string> files = directoryInfo.GetFiles().Select(f=>f.Name).ToList();
            return files;
        }

        public bool HasFile(string pathOrContainerName, string fileName)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, pathOrContainerName, fileName);
            return File.Exists(path);
            
        }

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, pathOrContainerName);
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            List<(string fileName, string pathOrContainerName)> datas = new();
            foreach(IFormFile file in files)
            {
                string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string fullPath = Path.Combine(path, newFileName);
                using FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: true);
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                datas.Add((newFileName, pathOrContainerName));
            }
            return datas;
        }
    }
}
