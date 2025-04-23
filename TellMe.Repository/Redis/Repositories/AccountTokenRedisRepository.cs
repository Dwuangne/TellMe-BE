using Redis.OM.Searching;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Redis.Models;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace TellMe.Repository.Redis.Repositories
{
    public class AccountTokenRedisRepository : IAccountTokenRedisRepository
    {
        private RedisConnectionProvider _redisConnectionProvider;
        private IRedisCollection<AccountToken> _accounttokenCollection;
        public AccountTokenRedisRepository(IConfiguration configuration)
        {
            var redisConnectionString = configuration["Redis:ConnectionString"]
                ?? throw new InvalidOperationException("Redis connection string is not configured.");
            _redisConnectionProvider = new RedisConnectionProvider(redisConnectionString);
            _accounttokenCollection = _redisConnectionProvider.RedisCollection<AccountToken>();
        }

        public async Task AddAccountToken(AccountToken accountToken)
        {
            try
            {
                await _accounttokenCollection.InsertAsync(accountToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AccountToken?> GetAccountToken(string accountId)
        {
            try
            {
                return await _accounttokenCollection.SingleOrDefaultAsync(x => x.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAccountToken(AccountToken accountToken)
        {
            try
            {
                await _accounttokenCollection.UpdateAsync(accountToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAccountToken(AccountToken accountToken)
        {
            try
            {
                await _accounttokenCollection.DeleteAsync(accountToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
