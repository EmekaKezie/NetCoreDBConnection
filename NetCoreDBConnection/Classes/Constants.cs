using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDBConnection.Classes
{
    public class Constants
    {
        public struct ResCode
        {
            public const string Success = "SUCCESS";
            public const string Failed = "FAILED";
            public const string AlreadyExists = "ALREADY_EXISTS";
        }

        public struct ResDesc
        {
            public const string Success = "SUCCESS";
            public const string Failed = "FAILED";
            public const string AlreadyExists = "ALREADY EXISTS";
        }
    }
}
