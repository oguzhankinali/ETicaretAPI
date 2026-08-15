using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly IProductImageFileWriteRepository _productImageFileWriteRepository;


        public ProductsController(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository, IWebHostEnvironment webHostEnvironment, IProductImageFileWriteRepository productImageFileWriteRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _webHostEnvironment = webHostEnvironment;
            _productImageFileWriteRepository = productImageFileWriteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] Pagination pagination)
        {
            int totalCount = await _productReadRepository.GetAll(false).CountAsync();
            var products = await _productReadRepository.GetAll(false)
                .Skip((pagination.Page - 1) * pagination.Size)
                .Take(pagination.Size)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Stock,
                    p.Price,
                    p.CreatedDate
                }).ToListAsync();
            return Ok(new
            {
                TotalCount = totalCount,
                Products = products
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productReadRepository.GetByIdAsync(id, false);
            return Ok(product);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            await _productWriteRepository.AddAsync(product);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
             _productWriteRepository.Update(product);
            await _productWriteRepository.SaveAsync();
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload([FromQuery] string id)
        {
            Product product = await _productReadRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound("Ürün bulunamadı!");
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "resource/product-images");
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Random random = new Random();
            List<ProductImageFile> datas = new List<ProductImageFile>();
            foreach (IFormFile file in Request.Form.Files)
            {
                string fileName = file.FileName;
                string newFileName = $"{random.Next()}{Path.GetExtension(fileName)}";
                string fullPath = Path.Combine(path, newFileName);
                using FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                datas.Add(new ProductImageFile { Path = "resource/product-images", FileName = newFileName, Storage = "Local" });
            }
            if (product.ProductImageFiles == null)
                product.ProductImageFiles = new List<ProductImageFile>();
            foreach (var image in datas)
            {
                product.ProductImageFiles.Add(image);
            }
            await _productWriteRepository.SaveAsync();
            return Ok();
        }



    }
}
