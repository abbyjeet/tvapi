using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tv.api.Common.Constants
{
    public static class Misc
    {
        public const int PAGESIZE = 9;

        public static int CurrentTimeStamp
        {
            get
            {
                return (int)DateTime.UtcNow.Subtract(DateTimeOffset.UnixEpoch.UtcDateTime).TotalSeconds;
            }
        }
    }
}
