using AuthServer.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Helper
{
    public interface IHelperFunction
    {
        object GenerateJwtToken(string email, WebOwner user, string Role);
    }
}
