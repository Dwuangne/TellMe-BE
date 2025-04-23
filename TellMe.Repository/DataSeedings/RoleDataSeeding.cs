using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TellMe.Repository.DataSeedings
{
    public static class RoleDataSeeding
    {
        private const string UserRoleId = "62f344b0-8cc9-4e78-9047-57af74c367ac";
        private const string PsychologistRoleId = "8b139abe-60d1-4f23-93e7-6e3f03c13fa3";
        private const string SupporterRoleId = "42e9bd6c-a78c-4321-9440-4c63573163f7";
        private const string AdminRoleId = "b4fefe26-a899-4611-90de-cdf797927add";
        public static void RoleData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = UserRoleId, ConcurrencyStamp = UserRoleId, Name = "User", NormalizedName = "User".ToUpper() },
                new IdentityRole() { Id = PsychologistRoleId, ConcurrencyStamp = PsychologistRoleId, Name = "Psychologist", NormalizedName = "Psychologist".ToUpper() },
                new IdentityRole() { Id = SupporterRoleId, ConcurrencyStamp = SupporterRoleId, Name = "Supporter", NormalizedName = "Supporter".ToUpper() },
                new IdentityRole() { Id = AdminRoleId, ConcurrencyStamp = AdminRoleId, Name = "Admin", NormalizedName = "Admin".ToUpper() }
            );
        }
    }
}
