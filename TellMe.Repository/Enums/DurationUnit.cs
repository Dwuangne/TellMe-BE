using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Repository.Enums
{
    /// <summary>
    /// Time unit applied to a subscription package.
    /// </summary>
    public enum DurationUnit
    {
        /// <summary>
        /// Day – for packages measured in days
        /// </summary>
        Day,

        /// <summary>
        /// Month – for packages measured in months
        /// </summary>
        Month,

        /// <summary>
        /// Year – for packages measured in years
        /// </summary>
        Year
    }
}
