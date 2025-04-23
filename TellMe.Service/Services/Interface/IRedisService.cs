using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Redis.Models;
using TellMe.Repository.Redis.Repositories;
using TellMe.Repository.SMTPs.Repositories;

namespace TellMe.Service.Services.Interface
{
    public interface IRedisService
    {
        Task AddAccountTokenAsync(AccountToken accountToken);
        Task<AccountToken> GetAccountTokenAsync(string accountId);
        Task UpdateAccountTokenAsync(AccountToken accountToken);
        Task DeleteAccountTokenAsync(AccountToken accountToken);
    }
}
