using ETicaretAPI.Application.Abstraction.Services.User;
using ETicaretAPI.Application.DTOs.User.CreateUser;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserResponseDTO> CreateUserAsync(CreateUserDTO user)
        {

          IdentityResult result =  await _userManager.CreateAsync(new() {
            Id = Guid.NewGuid().ToString(),
            NameSurname = user.NameSurname,
            Email = user.Email,
            UserName = user.Username},
            user.Password);

            if (result.Succeeded)
            {
                return new()
                {
                    Succeeded = true,
                    Message = "Kullanıcı başarıyla oluşturulmuştur."
                };
            }
            throw new UserCreatedFailedException();
        }
        
    }
}
