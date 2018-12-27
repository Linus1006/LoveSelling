using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoveSelling.Models
{
    [Flags]
    public enum AccountRank
    {
        User = 1,
        Saler = 2,
        Maintainer = 4,
        Manager = 8
    }
}