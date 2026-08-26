using ETicaretAPI.Application.DTOs.User.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Services.User
{
    public interface IUserService
    {
        Task<CreateUserResponseDTO> CreateUserAsync(CreateUserDTO user);

    }
}
