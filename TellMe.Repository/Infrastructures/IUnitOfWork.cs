using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Repository.Repositories;
using TellMe.Repository.SMTPs.Repositories;

namespace TellMe.Repository.Infrastructures
{
    public interface IUnitOfWork
    {
        public void Commit();
        public Task CommitAsync();
    }
}
