using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pozadavky.Services
{
    public static class Constants
    {
        public static bool Test { get; private set; } = false;

        public static bool TestEmaily { get; private set; } = false;

        public static string ActiveUser {
            get { return UserServices.GetActiveUser(); }
            private set { } 
        }

        public static List<int> ActiveUserLevel {
            get {return UserServices.GetActiveUserLevels(); }
            private set { } 
        }

    }
}