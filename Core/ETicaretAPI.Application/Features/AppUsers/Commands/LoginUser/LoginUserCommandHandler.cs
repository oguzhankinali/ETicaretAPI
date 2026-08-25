using ETicaretAPI.Application.Abstraction.Services.Authentications;
using ETicaretAPI.Application.Abstraction.Token;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Application.Features.AppUsers.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        private readonly IAuthService _authService;


        public LoginUserCommandHandler( IAuthService authService)
        {
         
            _authService = authService;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            Application.DTOs.Token token = await _authService.LoginAsync(request.UsernameOrEmail, request.Password, 900);
            var response = new LoginUserSuccessCommandResponse()
            {
                Token = token
            };
            return response;
        }
    }
}