using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Redis.Models;

namespace TellMe.Repository.Redis.Repositories
{
    public interface IAccountTokenRedisRepository
    {
        Task AddAccountToken(AccountToken accountToken);
        Task<AccountToken?> GetAccountToken(string accountId);
        Task UpdateAccountToken(AccountToken accountToken);
        Task DeleteAccountToken(AccountToken accountToken);

    }
}
