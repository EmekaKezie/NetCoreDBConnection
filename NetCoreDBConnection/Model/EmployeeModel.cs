using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreDBConnection.Model
{
    public class EmployeeModel
    {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class EmployeeModelRes
    {
        public EmployeeModel Data { get; set; }
        public ResponseModel Status { get; set; }
    }

    public class EmployeeBulkRes
    {
        public List<EmployeeModel> Data { get; set; }
        public ResponseModel Status { get; set; }
    }

    public class Employee
    {
        public string EmployeeId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class EmployeeListRes
    {
        public List<Employee> Data { get; set; }
        public ResponseModel Status { get; set; }
    }
}
