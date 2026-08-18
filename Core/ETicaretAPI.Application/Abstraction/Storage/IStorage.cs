using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ETicaretAPI.Application.Abstraction.Storage
{
    public interface IStorage
    {
        Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files);

        Task DeleteAsync(string pathOrContainerName, string fileName);

        Task<List<string>> GetFiles(string pathOrContainerName);

        Task<bool> HasFile(string pathOrContainerName, string fileName);
    }
}
