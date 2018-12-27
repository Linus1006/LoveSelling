using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoveSelling.Models
{
    public enum SignInStatus
    {
        Success = 0,
        LockedOut = 1,
        RequiresVerification = 2,
        Failure = 3,
        Logout = 4
    }
}