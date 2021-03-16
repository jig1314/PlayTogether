using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Services
{
    public interface IUnauthorizedUserService
    {
        Task<List<Gender>> GetGenders();
        Task<List<Country>> GetCountries();
        Task RegisterNewUser(RegisterUserDto registerUserDto);
        Task Login(LoginDto loginDto);
        Task ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
