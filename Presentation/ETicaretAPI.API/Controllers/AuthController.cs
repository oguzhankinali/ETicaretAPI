using ETicaretAPI.Application.Features.AppUsers.Commands.GoogleLogin;
using ETicaretAPI.Application.Features.AppUsers.Commands.LoginUser;
using ETicaretAPI.Application.Features.AppUsers.Commands.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginUserCommandRequest googleLoginUserCommandRequest)
        {
            GoogleLoginUserCommandResponse googleLoginUserCommandResponse = await _mediator.Send(googleLoginUserCommandRequest);
            return Ok(googleLoginUserCommandResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommandRequest loginUserCommandRequest)
        {
            LoginUserCommandResponse response = await _mediator.Send(loginUserCommandRequest);
            return Ok(response);
        }

        [HttpPost("refresh-token-login")]
        public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenCommandRequest refreshTokenCommandRequest)
        {
           RefreshTokenCommandResponse response = await _mediator.Send(refreshTokenCommandRequest);
           return Ok(response);
        }
    }
}
