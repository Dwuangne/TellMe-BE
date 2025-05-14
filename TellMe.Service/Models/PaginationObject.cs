using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Models
{
    public class PaginationObject
    {
        public int TotalRecord { get; set; }
        public int TotalPage { get; set; }
        public int PageIndex { get; set; }
        public dynamic Data { get; set; }
    }
}
