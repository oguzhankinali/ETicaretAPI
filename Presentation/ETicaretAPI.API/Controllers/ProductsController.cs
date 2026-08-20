using ETicaretAPI.Application.Abstraction.Storage;
using ETicaretAPI.Application.Features.Products.Commands.CreateProduct;
using ETicaretAPI.Application.Features.Products.Commands.RemoveProduct;
using ETicaretAPI.Application.Features.Products.Commands.UpdateProduct;
using ETicaretAPI.Application.Features.Products.Queries.GetAllProduct;
using ETicaretAPI.Application.Features.Products.Queries.GetByIdProduct;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Files;
using ETicaretAPI.Persistance.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;


        private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        private readonly IStorageService _storageService;
        private readonly IMediator _mediator;


        public ProductsController(IMediator mediator, IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IStorageService storageService)
        {
            _mediator = mediator;
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] GetByIdProductQueryRequest getByIdProductQueryRequest)
        {
            GetByIdProductQueryResponse getByIdProductQueryResponse = await _mediator.Send(getByIdProductQueryRequest);
            if (getByIdProductQueryResponse == null)
                return NotFound();
            return Ok(getByIdProductQueryResponse);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] RemoveProductCommandRequest removeProductCommandRequest)
        {
            RemoveProductCommandResponse response = await _mediator.Send(removeProductCommandRequest);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
        {
            UpdateProductCommandResponse updateProductCommandResponse = await _mediator.Send(updateProductCommandRequest);
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload([FromQuery] string id)
        {
            Product product = await _productReadRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound("Ürün bulunamadı!");
            List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("product-images", Request.Form.Files);
            var datas = result.Select(d => new ProductImageFile
            {
                FileName = d.fileName,
                Path = d.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products = new List<Product> { product }
            }).ToList();
            await _productImageFileWriteRepository.AddRangeAsync(datas);
            await _productImageFileWriteRepository.SaveAsync();
            return Ok();
        }
        [HttpGet("[Action]/{id}")]
        public async Task<IActionResult> GetProductImages([FromRoute] string id)
        {
            var product = await _productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));
            if (product == null)
                return NotFound();
            return Ok(product.ProductImageFiles.Select(p => new { p.Path, p.FileName, p.Id }));

        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] string id, [FromQuery] string imageId)
        {
            var product = await _productReadRepository.Table.Include(p => p.ProductImageFiles).FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));
            if (product == null)
                return NotFound();
            ProductImageFile? productImageFile = product.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(imageId));
            if (productImageFile == null)
                return NotFound();
            await _storageService.DeleteAsync(productImageFile.Path, productImageFile.FileName);
            product.ProductImageFiles.Remove(productImageFile);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }



    }
}