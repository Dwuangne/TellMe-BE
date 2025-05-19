using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Services.Interface
{
    public interface ITimeHelper
    {
        DateTime ToVietnamTime(DateTime utcTime);
        DateTime NowVietnam();
        DateTime NormalizeToVietnam(DateTime input);
    }

}
