using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.AppUsers.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public CreateUserCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            IdentityResult result= await _userManager.CreateAsync(new()
            {
                UserName = request.Username,
                Email = request.Email,
                NameSurname = request.NameSurname
            }, request.Password);
            if (result.Succeeded)
                return new()
                {
                    Succeeded = true,
                    Message = "Kullanıcı başarıyla oluşturulmuştur."
                };
            throw new UserCreatedFailedException();
            

        }
    }
}
