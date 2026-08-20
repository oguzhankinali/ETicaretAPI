using ETicaretAPI.Application.Abstraction.Storage;
using ETicaretAPI.Application.Features.Products.Commands.CreateProduct;
using ETicaretAPI.Application.Features.Products.Commands.DeleteProductImage;
using ETicaretAPI.Application.Features.Products.Commands.DeleteProductImages;
using ETicaretAPI.Application.Features.Products.Commands.RemoveProduct;
using ETicaretAPI.Application.Features.Products.Commands.UpdateProduct;
using ETicaretAPI.Application.Features.Products.Commands.UploadProductImage;
using ETicaretAPI.Application.Features.Products.Queries.GetAllProduct;
using ETicaretAPI.Application.Features.Products.Queries.GetByIdProduct;
using ETicaretAPI.Application.Features.Products.Queries.GetProductImages;
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
        private readonly IMediator _mediator;


        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
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
            await _mediator.Send(new UploadProductImageCommandRequest 
            { 
            Id = id,
            Files = Request.Form.Files
            });
            return StatusCode((int)HttpStatusCode.Created);
            
        }
        [HttpGet("[Action]/{Id}")]
        public async Task<IActionResult> GetProductImages([FromRoute] GetProductImagesQueryRequest getProductImagesQueryRequest)
        {
            List<GetProductImagesQueryResponse> getProductImagesQueryResponse = await _mediator.Send(getProductImagesQueryRequest);
            return Ok(getProductImagesQueryResponse);

        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] string id, [FromQuery] string imageId)
        {

            DeleteProductImageCommandResponse response = await _mediator.Send(new DeleteProductImageCommandRequest
            {
                Id = id,
                ImageId = imageId
            });

            return Ok();
        }



    }
}