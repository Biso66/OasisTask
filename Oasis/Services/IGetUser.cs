using Microsoft.AspNetCore.Mvc;
using Oasis.Models;

namespace Oasis.Services
{
    public interface IGetUser
    {
        Task<int> GetUserByToken(string token);
    }
}
