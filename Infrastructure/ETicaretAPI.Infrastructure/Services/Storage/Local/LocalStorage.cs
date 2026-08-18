using ETicaretAPI.Application.Abstraction.Storage;
using ETicaretAPI.Infrastructure.Operations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
                string newFileName = await FileRenameAsync(path, file.FileName);
                string fullPath = Path.Combine(path, newFileName);
                using FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: true);
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                datas.Add((newFileName, pathOrContainerName));
            }
            return datas;
        }
        private async Task<string> FileRenameAsync(string path, string fileName, bool first = true)
        {
            string extension = Path.GetExtension(fileName);
            string oldName = Path.GetFileNameWithoutExtension(fileName);
            string cleanName = "";
            
            if (first)
            {
                cleanName =NameOperation.CharacterRegulatory(oldName);            
            }
            else
            {
                cleanName = oldName;
            }
            string newFileName = $"{cleanName}{extension}";
            if (File.Exists(Path.Combine(path, newFileName)))
            {
                int lastIndex = cleanName.LastIndexOf("-");
                if(lastIndex == -1)
                {
                    return await FileRenameAsync(path, $"{cleanName}-1{extension}", false);
                }
                else
                {
                    string lastPart = cleanName.Substring(lastIndex + 1);

                    if (int.TryParse(lastPart, out int index)){
                        index++;
                        cleanName = $"{cleanName.Substring(0, lastIndex)}-{index}";
                        return await FileRenameAsync(path, $"{cleanName}{extension}", false);
                    }
                    else
                    {
                        cleanName = $"{cleanName}-1";
                        return await FileRenameAsync(path, $"{cleanName}{extension}", false);
                    }
                }
            }
            else
                return newFileName;
        }
    }
}
