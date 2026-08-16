using ETicaretAPI.Domain.Entities.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories
{
    public interface IProductImageFileReadRepository : IReadRepository<ProductImageFile>
    {
    }
}
