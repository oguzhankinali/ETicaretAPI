using ETicaretAPI.Application.DTOs.User.CreateUser;
using ETicaretAPI.Domain.Entities.Identity;
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
        Task UpdateRefreshToken(string refreshToken, AppUser user, DateTime createdTime, int addTime);

    }
}
