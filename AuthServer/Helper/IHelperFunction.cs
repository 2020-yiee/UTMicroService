using AuthServer.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Helper
{
    public interface IHelperFunction
    {
        object GenerateJwtToken(string email, User user, string Role);
        object GenerateJwtToken(string email, Admin user, string Role);
        String RandomPassword();
        bool sendMail(string email, string fullname, string password);
    }
}
