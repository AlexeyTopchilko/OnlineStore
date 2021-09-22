using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Repository.Repository;
using AuthenticationMicroservice.Service.Models;
using AuthenticationMicroservice.Service.Services.Encryption;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationMicroservice.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IRepositoryGeneric<User> _userRepository;

        public UserService(IRepositoryGeneric<User> userRepository)
        {
            _userRepository = userRepository;
        }

        #region CRUD
        public async Task<User> GetUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user;
        }

        public async Task<User> GetUserByName(string name)
        {
            var user = (await _userRepository.GetByPredicate(_ => _.Username == name)).FirstOrDefault();
            return user;
        }
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var user = await _userRepository.GetAllAsync();
            return user;
        }

        public async Task AddAsync(RegisterModel model)
        {
            if (await IsEmailExist(model.Email) || await IsUsernameExist(model.Username))
                throw new InvalidOperationException();

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password.EncrypteString()
            };
            await _userRepository.CreateAsync(user);
        }

        public async Task RemoveUserAsync(User user, Guid id)
        {
            await _userRepository.DeleteAsync(user, id);
        }

        #endregion


        #region Validations
        public async Task<bool> IsUsernameExist(string username)
        {
            return (await _userRepository.GetByPredicate(_ => _.Username == username)).Any();

        }

        public async Task<bool> IsEmailExist(string email)
        {
            return (await _userRepository.GetByPredicate(_ => _.Email == email)).Any();

        }
        #endregion

        #region Token
        public JwtSecurityToken CreateToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            JwtSecurityToken jwt = new(
                   issuer: AuthOptions.Issuer,
                   audience: AuthOptions.Audience,
                   notBefore: now,
                   claims: identity.Claims,
                   expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
                   signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return jwt;
        }

        public string EncodeJwt(JwtSecurityToken jwt)
        {
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public async Task<ClaimsIdentity> GetIdentityAsync(LoginModel model)
        {
            var user = (await _userRepository.GetByPredicate(_ => _.Username == model.Username && _.Password == model.Password.EncrypteString() && _.DeletedDate == null)).FirstOrDefault();
            if (user == null) return null;
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            ClaimsIdentity claimsIdentity =
                new(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
        #endregion
    }
}