using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ETicaretAPI.Application.Abstraction.Storage;
using ETicaretAPI.Infrastructure.Operations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage.Azure
{
    public class AzureStorage : Storage, IAzureStorage
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorage(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["Storage:Azure"]);
        }

        public async Task DeleteAsync(string pathOrContainerName, string fileName)
        {
            BlobContainerClient _blobContainerClient = _blobServiceClient.GetBlobContainerClient(pathOrContainerName);
            await _blobContainerClient.DeleteBlobIfExistsAsync(fileName);
        }

        public async Task<List<string>> GetFiles(string pathOrContainerName)
        {
            List<string> _items = new();
            BlobContainerClient _blobContainerClient = _blobServiceClient.GetBlobContainerClient(pathOrContainerName);
            await foreach (BlobItem item in _blobContainerClient.GetBlobsAsync())
            {
                _items.Add(item.Name);
            }
            return _items;
        }

        public async Task<bool> HasFile(string pathOrContainerName, string fileName)
        {
            BlobContainerClient _blobContainerClient = _blobServiceClient.GetBlobContainerClient(pathOrContainerName);
            BlobClient _blobClient = _blobContainerClient.GetBlobClient(fileName);
            return await _blobClient.ExistsAsync();
        }

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files)
        {
            BlobContainerClient _blobContainerClient = _blobServiceClient.GetBlobContainerClient(pathOrContainerName);

            await _blobContainerClient.CreateIfNotExistsAsync();
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
            List<(string fileName, string pathOrContainerName)> datas = new();
            foreach (IFormFile file in files)
            {
                string newFileName = await FileRenameAsync(pathOrContainerName, file.FileName, HasFile);
                await _blobContainerClient.UploadBlobAsync(newFileName, file.OpenReadStream());
                datas.Add((newFileName, pathOrContainerName));
            }
            return datas;
        }
    }
}
