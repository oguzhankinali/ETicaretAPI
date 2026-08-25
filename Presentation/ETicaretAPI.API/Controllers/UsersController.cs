using ETicaretAPI.Application.Features.AppUsers.Commands.CreateUser;
using ETicaretAPI.Application.Features.AppUsers.Commands.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task <IActionResult> CreateUser([FromBody] CreateUserCommandRequest createUserCommandRequest)
        {
            CreateUserCommandResponse createUserCommandResponse = await _mediator.Send(createUserCommandRequest);
            return Ok(createUserCommandResponse);
        }


   
    }
}
