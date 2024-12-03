using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories.Interfaces
{
    public interface IClassWiseStudentRepository
    {
        public List<Student.StudentDetailsWithFees> StudentDetailsWithFees(int id);
        public int DeleteStudentDetailsWithFees(int id);

    }
}