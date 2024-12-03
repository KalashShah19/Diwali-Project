using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;


namespace WebAPI.Repositories;

interface IRegisterRepository
{
     Task<bool> RegisterUser(User.Register registerModel );
     Task<bool> RoleStudent(string email, string standard);
     //     Task<bool> RoleStudent(Student.StudentDetails registerModel);
     Task<bool> RoleTeacher(string email, string standard, string qualification);   
     // Task<bool> Forgot(string email, string standard, string qualification);   
}
