﻿using AuthServer.EFModels;
using AuthServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Repository
{
    public interface IWebOwnerRepository
    {
        WebOwner getCustomerByUsernameAndPassword(LoginRequestModel model);

    }
}