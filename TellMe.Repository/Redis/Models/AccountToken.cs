using Microsoft.EntityFrameworkCore;
using Redis.OM.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Redis.Models
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "AccountToken" }, IndexName = "account_tokens")]
    public class AccountToken
    {
        [RedisIdField]
        [Indexed]
        public string AccountId { get; set; } = string.Empty;
        [Indexed]
        public string JWTId { get; set; } = string.Empty;
        [Indexed]
        public string RefreshToken { get; set; } = string.Empty;
        [Indexed]
        public DateTime ExpiredDate { get; set; } = DateTime.Now.AddDays(7);
    }
}
