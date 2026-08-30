using ETicaretAPI.Application.Abstraction.Services.Authentications;
using ETicaretAPI.Application.Abstraction.Services.User;
using ETicaretAPI.Application.Abstraction.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;


namespace ETicaretAPI.Persistance.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserService _userService;
        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, IConfiguration configuration, SignInManager<AppUser> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _signInManager = signInManager;
            _userService = userService;
        }


        public async Task<Application.DTOs.Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:ClientId"]! }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");

            AppUser? user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            bool result = user != null;
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        NameSurname = payload.Name
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
                await _userManager.AddLoginAsync(user, info);
            else
                throw new Exception("Geçersiz harici kimlik doğrulama.");
            Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime, user);
            await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 60);
            return token;
        }

        public async Task<Application.DTOs.Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            AppUser? user = await _userManager.FindByEmailAsync(usernameOrEmail);
            if(user == null)
            {
                user = await _userManager.FindByNameAsync(usernameOrEmail);
                if(user==null)
                    throw new NotFoundUserException("Kullanıcı adı veya şifre hatalı.");
            }
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (result.Succeeded)
            {
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime, user);
                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 60);
                return token;
            }
            throw new NotFoundUserException("Kullanıcı adı veya şifre hatalı.");
        }

        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenEndDate > DateTime.UtcNow);
            if (user == null)
                throw new NotFoundUserException("Geçersiz Refresh Token.");
            var newToken = _tokenHandler.CreateAccessToken(15, user);
            await _userService.UpdateRefreshToken(newToken.RefreshToken, user, newToken.Expiration, 60);
            return newToken;
        }
    }
}
