using ETicaretAPI.Application.Abstraction.Services.Authentications;
using ETicaretAPI.Application.Abstraction.Token;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace ETicaretAPI.Persistance.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        public AuthService(UserManager<AppUser> userManager, ITokenHandler tokenHandler, IConfiguration configuration, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _signInManager = signInManager;
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
           return _tokenHandler.CreateAccessToken(accessTokenLifeTime, user);
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
                return _tokenHandler.CreateAccessToken(accessTokenLifeTime, user);
            }
            throw new NotFoundUserException("Kullanıcı adı veya şifre hatalı.");
        }
    }
}
