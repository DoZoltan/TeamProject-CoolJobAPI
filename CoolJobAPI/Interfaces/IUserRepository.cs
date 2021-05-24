﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoolJobAPI.Models
{
    public interface IUserRepository
    {
        bool DeleteUserByName(string userName);
        Task<bool> RegisterNewUser(User newUser);
    }
}
