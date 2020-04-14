using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Auth.Common
{
    public static class AppSettings
    {
        public static string SecretKey { get; set; }
        public static long ExpiredTime { get; set; }
        public static string GoogleClientId { get; set; }
        public static string GoogleClientSecret { get; set; }
        public static string GoogleRedirectUri { get; set; }
    }
}
