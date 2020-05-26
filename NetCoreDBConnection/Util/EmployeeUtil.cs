using NetCoreDBConnection.Classes;
using NetCoreDBConnection.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using static NetCoreDBConnection.Classes.Constants;

namespace NetCoreDBConnection.Util
{
    public class EmployeeUtil
    {
        public static EmployeeListRes GetEmployee()
        {
            EmployeeListRes res = new EmployeeListRes();
            List<Employee> list = new List<Employee>();
            ResponseModel status = new ResponseModel();
            DbProviderFactory dpf = DbFactory.DbProviderFactory();

            try
            {
                using (DbConnection con = dpf.CreateConnection())
                {
                    con.ConnectionString = DbFactory.Connection;
                    using (DbCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = Sql.GetEmployee;
                        con.Open();
                        using (DbDataReader r = cmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                while (r.Read())
                                {
                                    list.Add(new Employee
                                    {
                                        Firstname = r["firstname"].ToString(),
                                        Lastname = r["lastname"].ToString(),
                                    });
                                }
                                status.Code = ResCode.Success;
                                status.Desc = ResDesc.Success;
                                res.Data = list;
                                res.Status = status;
                            }
                        }
                        con.Close();
                    }
                    
                }
            }
            catch (Exception)
            {
                throw;
            }

            return res;
        }


        public static EmployeeModelRes CreateEmployee(EmployeeModel model)
        {
            EmployeeModelRes res = new EmployeeModelRes();
            ResponseModel status = new ResponseModel();

            object[] exists = { model.Username };
            int count = DbFactory.ExecCount(Sql.CheckExistsEmployee, exists);
            if (DbFactory.ExecCount(Sql.CheckExistsEmployee, exists) == 0)
            {
                string staffId = Guid.NewGuid().ToString();
                string password = model.Username;
                object[] param = { staffId, model.Username, password, model.Firstname, model.Lastname, DateTime.Now };
                if (DbFactory.ExecCommand(Sql.CreateEmployee, param) > 0)
                {
                    status.Code = ResCode.Success;
                    status.Desc = ResDesc.Success;
                    res.Data = model;
                    res.Status = status;
                }
                else
                {
                    status.Code = ResCode.Failed;
                    status.Desc = ResDesc.Failed;
                    res.Status = status;
                }
            }
            else
            {
                status.Code = ResCode.AlreadyExists;
                status.Desc = ResDesc.AlreadyExists;
                res.Status = status;
            }

            return res;
        }


        public static EmployeeBulkRes CreateBulkEmployee(List<EmployeeModel> model)
        {
            EmployeeBulkRes res = new EmployeeBulkRes();
            ResponseModel status = new ResponseModel();

            int count = model.Count;
            object[] param = new object[count];
            string[] sql = new string[count];


            sql[0] = Sql.CreateEmployee;
            object[] p1 = { "6", "john", "john", "john", "john", DateTime.Now };
            param[0] = p1;

            sql[1] = Sql.CreateEmployee;
            object[] p2 = { "7", "john", "john", "john", "john", DateTime.Now };
            param[1] = p2;

            int done = DbFactory.ExecCommandTranx(sql, param);

            return res;
        }
    }
}
