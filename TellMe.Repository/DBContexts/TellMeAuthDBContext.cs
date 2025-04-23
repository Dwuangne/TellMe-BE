using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TellMe.Repository.DataSeedings;
using TellMe.Repository.Enities;

namespace TellMe.Repository.DBContexts
{
    public class TellMeAuthDBContext : IdentityDbContext<ApplicationUser>
    {
        public TellMeAuthDBContext(DbContextOptions<TellMeAuthDBContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.RoleData();
        }
    }
}
