using ETicaretAPI.Application.Abstraction.Services.Authentications;
using ETicaretAPI.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.AppUsers.Commands.GoogleLogin
{
    public class GoogleLoginUserCommandHandler : IRequestHandler<GoogleLoginUserCommandRequest, GoogleLoginUserCommandResponse>
    {
        private readonly IAuthService _authService;

        public GoogleLoginUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<GoogleLoginUserCommandResponse> Handle(GoogleLoginUserCommandRequest request, CancellationToken cancellationToken)
        {
           Token token = await _authService.GoogleLoginAsync(request.idToken, 900);
            var response = new GoogleLoginUserCommandResponse()
            {
                accessToken = token
            };
            return response;
        }
    }
}
