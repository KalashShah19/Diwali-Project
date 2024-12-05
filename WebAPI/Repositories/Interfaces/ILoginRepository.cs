using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories;

     interface ILoginRepository
    {
         (int?,string?) ValidateUser(Login Login);
         (string,string) RequestPasswordChange(string email);
         string? Newpassword(Forgotpassword Forgotpassword);
    }


// using WebAPI.Models;

// namespace WebAPI.Repositories;

// interface ILoginRepository
// {
//     bool ValidateUser(User.Register loginModel);
// }

