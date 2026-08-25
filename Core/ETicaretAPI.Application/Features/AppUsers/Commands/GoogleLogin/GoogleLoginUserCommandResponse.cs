using ETicaretAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.AppUsers.Commands.GoogleLogin
{
    public class GoogleLoginUserCommandResponse
    {
        public Token accessToken { get; set; }
    }
}
