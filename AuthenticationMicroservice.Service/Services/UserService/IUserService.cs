using AuthenticationMicroservice.Domain.Entities;
using AuthenticationMicroservice.Service.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.Service.Services.UserService
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();

        Task<User> GetUserAsync(Guid id);

        Task<User> GetUserByName(string name);

        Task RemoveUserAsync(User user, Guid id);

        Task<ClaimsIdentity> GetIdentityAsync(LoginModel model);

        Task AddAsync(RegisterModel model);

        Task<bool> IsUsernameExist(string username);

        Task<bool> IsEmailExist(string email);

        JwtSecurityToken CreateToken(ClaimsIdentity identity);

        string EncodeJwt(JwtSecurityToken jwt);
    }
}