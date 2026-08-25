using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.AppUsers.Commands.GoogleLogin
{
    public class GoogleLoginUserCommandRequest : IRequest<GoogleLoginUserCommandResponse>
    {
        public string idToken { get; set; }
    }
}
