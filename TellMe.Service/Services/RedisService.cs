using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Redis.Models;
using TellMe.Repository.Redis.Repositories;
using TellMe.Service.Constants;
using TellMe.Service.Services.Interface;

namespace TellMe.Service.Services
{
    public class RedisService : IRedisService
    {
        private readonly IAccountTokenRedisRepository _accountTokenRedisRepository;
        public RedisService(IAccountTokenRedisRepository accountTokenRedisRepository)
        {
            _accountTokenRedisRepository = accountTokenRedisRepository;
        }
        public async Task AddAccountTokenAsync(AccountToken accountToken)
        {
            if (accountToken == null)
                throw new ArgumentNullException(nameof(accountToken));

            if (string.IsNullOrWhiteSpace(accountToken.AccountId))
                throw new ArgumentException(nameof(accountToken.AccountId));

            if (accountToken.ExpiredDate <= DateTime.UtcNow)
                throw new ArgumentException(nameof(accountToken.ExpiredDate));

            try
            {
                await _accountTokenRedisRepository.AddAccountToken(accountToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<AccountToken> GetAccountTokenAsync(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException(nameof(accountId));

            try
            {
                var accountToken = await _accountTokenRedisRepository.GetAccountToken(accountId);
                if (accountToken == null)
                    throw new 
                        
                        (MessageConstant.Cache.AccountTokenNotFound);

                return accountToken;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task UpdateAccountTokenAsync(AccountToken accountToken)
        {
            if (accountToken == null)
                throw new ArgumentNullException(nameof(accountToken));

            if (string.IsNullOrWhiteSpace(accountToken.AccountId))
                throw new ArgumentException(nameof(accountToken.AccountId));

            try
            {
                var existingToken = await _accountTokenRedisRepository.GetAccountToken(accountToken.AccountId);
                if (existingToken == null)
                    throw new InvalidOperationException(MessageConstant.Cache.AccountTokenNotFound);

                await _accountTokenRedisRepository.UpdateAccountToken(accountToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task DeleteAccountTokenAsync(AccountToken accountToken)
        {
            if (accountToken == null)
                throw new ArgumentNullException(nameof(accountToken));

            if (string.IsNullOrWhiteSpace(accountToken.AccountId))
                throw new ArgumentException(nameof(accountToken.AccountId));

            try
            {
                var existingToken = await _accountTokenRedisRepository.GetAccountToken(accountToken.AccountId);
                if (existingToken == null)
                    throw new InvalidOperationException(MessageConstant.Cache.AccountTokenNotFound);

                await _accountTokenRedisRepository.DeleteAccountToken(accountToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
