using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Products.Queries.GetByIdProduct
{
    public class GetByIdProductQueryResponse
    {
        
            public string Id { get; set; }
            public string Name { get; set; }
            public int Stock { get; set; }
            public float Price { get; set; }
            public object ProductImageFiles { get; set; }
        
    }
}
