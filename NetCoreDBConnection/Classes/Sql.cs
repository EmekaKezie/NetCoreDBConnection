using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDBConnection.Classes
{
    public class Sql
    {
        public const string CheckExistsEmployee = "SELECT COUNT(username) FROM emp_staff WHERE UPPER(username)=UPPER(@0)";
        public const string CreateEmployee = "INSERT INTO emp_staff (staff_id, username, password, firstname, lastname, entry_date) VALUES (@0, @1, @2, @3, @4, @5)";
        public const string GetEmployee = "select * from emp_staff";
    }
}
