using ETicaretAPI.Application.RequestParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Products.Queries.GetAllProduct
{
    public class GetAllProductQueryRequest :Pagination, IRequest<GetAllProductQueryResponse>
    {
    }
}
