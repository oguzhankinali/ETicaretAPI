using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Products.Queries.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
    {
        private readonly IProductReadRepository _productReadRepository;
        public GetAllProductQueryHandler(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }
        public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
            int totalCount = await _productReadRepository.GetAll(false).CountAsync(cancellationToken);

            var products = await _productReadRepository.GetAll(false)
                .Include(p => p.ProductImageFiles)
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Stock,
                    p.Price,
                    p.CreatedDate,
                    p.ProductImageFiles
                }).ToListAsync(cancellationToken);

            return new GetAllProductQueryResponse
            {
                TotalCount = totalCount,
                Products = products
            };
        }
    }
}
