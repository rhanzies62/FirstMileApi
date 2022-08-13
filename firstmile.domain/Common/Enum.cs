using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace firstmile.Domain.Common
{
    public enum Unauthorization
    {
        Expired,
        InvalidToken,
        Missing,
        InvalidAccess
    }

    public enum UserType
    {
        Admin = 1,
        User = 2
    }


}
