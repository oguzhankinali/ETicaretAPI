using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = ETicaretAPI.Domain.Entities.Files.File;


namespace ETicaretAPI.Application.Repositories
{
    public interface IFileWriteRepository : IWriteRepository<File>
    {
    }
}
