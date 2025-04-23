using Microsoft.Extensions.Configuration;
using System;
using System.Numerics;
using System.Threading.Tasks;
using TellMe.Repository.DBContexts;
using TellMe.Repository.Repositories;
using TellMe.Repository.SMTPs.Repositories;

namespace TellMe.Repository.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TellMeDBContext _dbContext;
        private readonly TellMeAuthDBContext _dbAuthContext;

        public UnitOfWork(TellMeDBContext dbContext, TellMeAuthDBContext dbAuthContext) 
        {
            _dbContext = dbContext;
            _dbAuthContext = dbAuthContext;

        }

        

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await Task.WhenAll(
                _dbContext.SaveChangesAsync()
            );
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
            _dbAuthContext?.Dispose();
        }
    }
}