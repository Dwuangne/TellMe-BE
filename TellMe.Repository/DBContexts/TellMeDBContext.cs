using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TellMe.Repository.DBContexts
{
    public class TellMeDBContext : DbContext
    {
        public TellMeDBContext()
        {
        }

        public TellMeDBContext(DbContextOptions<TellMeDBContext> options) : base(options)
        {
        }
    }
}
