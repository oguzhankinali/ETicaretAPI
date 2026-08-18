    using ETicaretAPI.Infrastructure.Operations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ETicaretAPI.Infrastructure.Services.Storage
    {
        public class Storage
        {
            protected async Task<string> FileRenameAsync(string pathOrContainerName, string fileName, Func<string, string, Task<bool>> hasFileMethod, bool first = true)
            {
                string extension = Path.GetExtension(fileName);
                string oldName = Path.GetFileNameWithoutExtension(fileName);
                string cleanName = "";

                if (first)
                {
                    cleanName = NameOperation.CharacterRegulatory(oldName);
                }
                else
                {
                    cleanName = oldName;
                }
                string newFileName = $"{cleanName}{extension}";
                if (await hasFileMethod(pathOrContainerName, newFileName))
                {
                    int lastIndex = cleanName.LastIndexOf("-");
                    if (lastIndex == -1)
                    {
                        return await FileRenameAsync(pathOrContainerName, $"{cleanName}-1{extension}", hasFileMethod,false);
                    }
                    else
                    {
                        string lastPart = cleanName.Substring(lastIndex + 1);

                        if (int.TryParse(lastPart, out int index))
                        {
                            index++;
                            cleanName = $"{cleanName.Substring(0, lastIndex)}-{index}";
                            return await FileRenameAsync(pathOrContainerName, $"{cleanName}{extension}", hasFileMethod, false);
                        }
                        else
                        {
                            cleanName = $"{cleanName}-1";
                            return await FileRenameAsync(pathOrContainerName, $"{cleanName}{extension}", hasFileMethod, false);
                        }
                    }
                }
                else
                    return newFileName;
            }
        }
    }
