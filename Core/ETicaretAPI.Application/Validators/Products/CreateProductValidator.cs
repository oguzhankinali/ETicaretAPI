using ETicaretAPI.Application.Features.Products.Commands.CreateProduct;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommandRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Ürün adı boş olamaz.")
                .MaximumLength(50)
                .MinimumLength(5)
                    .WithMessage("Ürün adı 5 ile 50 karakter arasında olmalıdır.");
            RuleFor(p => p.Price)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Ürün fiyatı boş olamaz.")
                .GreaterThan(0)
                    .WithMessage("Ürün fiyatı 0'dan büyük olmalıdır.");
            RuleFor(p => p.Stock)
                .NotEmpty()
                .NotNull()
                    .WithMessage("Ürün stok miktarı boş olamaz.")
                .GreaterThanOrEqualTo(0)
                    .WithMessage("Ürün stok miktarı 0'dan büyük veya eşit olmalıdır.");
        }
    }
}
