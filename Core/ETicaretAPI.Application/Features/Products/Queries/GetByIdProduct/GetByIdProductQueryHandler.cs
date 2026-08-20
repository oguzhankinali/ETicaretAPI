using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Products.Queries.GetByIdProduct
{
    public class GetByIdProductQueryHandler : IRequestHandler<GetByIdProductQueryRequest, GetByIdProductQueryResponse>
    {
        private readonly IProductReadRepository _productReadRepository;
        public GetByIdProductQueryHandler(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<GetByIdProductQueryResponse> Handle(GetByIdProductQueryRequest request, CancellationToken cancellationToken)
        {
            Product? product = await _productReadRepository.Table
            .Include(p => p.ProductImageFiles)
            .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.Id), cancellationToken);

            if (product == null)
                return null; 

            return new GetByIdProductQueryResponse()
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Stock = product.Stock,
                Price = product.Price,
                ProductImageFiles = product.ProductImageFiles.Select(pif => new
                {
                    pif.Id,
                    pif.FileName,
                    pif.Path
                }).ToList()
            };

        }
    }
}
