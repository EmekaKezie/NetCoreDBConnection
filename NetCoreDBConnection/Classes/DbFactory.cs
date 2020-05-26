using NetCoreDBConnection.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IsolationLevel = System.Data.IsolationLevel;

namespace NetCoreDBConnection.Classes
{
    public class DbFactory
    {

        //public const string Connection = "Server=127.0.0.1; Port=3306; Database=testdb; User Id=root";
        public const string Connection = "Server=127.0.0.1; Port=3306; Database=testdb; User Id=root";


        public static DbProviderFactory DbProviderFactory()
        {
            DbProviderFactory provider = null;
            DataSet ds = new DataSet();

            try
            {
                DataTable tb = new DataTable("system.data");
                tb.Columns.Add("InvariantName");
                tb.Columns.Add("Description");
                tb.Columns.Add("Name");
                tb.Columns.Add("AssemblyQualifiedName");
                ds.Tables.Add(tb);
                DataRow dr = ds.Tables[0].NewRow();

                //dr["InvariantName"] = "Npgsql";
                //dr["Description"] = ".Net Data Provider for PostgreSQL";
                //dr["Name"] = "Npgsql Data Provider";
                //dr["AssemblyQualifiedName"] = "Npgsql.NpgsqlFactory, Npgsql, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7";

                dr["InvariantName"] = "MySql.Data.MySqlClient";
                dr["Description"] = ".Net Framework Data Provider for MySQL";
                dr["Name"] = "MySQL Data Provider";
                dr["AssemblyQualifiedName"] = "MySql.Data.MySqlClient.MySqlClientFactory, MySql.Data, Version=6.7.2.0";


                provider = DbProviderFactories.GetFactory(dr);

            }
            catch (Exception)
            {
                throw;
            }

            return provider;
        }


        public static object ParamRead(object[] param, DbCommand cmd)
        {
            int len = param.Length;

            DbParameter p = null;
            for (int i = 0; i < len; i++)
            {
                p = cmd.CreateParameter();
                if (param[i].GetType() == typeof(int))
                {
                    p.DbType = DbType.Int32;
                    p.Value = Convert.ToInt32(param[i]);
                }
                else if (param[i].GetType() == typeof(string))
                {
                    p.DbType = DbType.String;
                    p.Value = param[i].ToString();
                }
                else if (param[i].GetType() == typeof(DateTime))
                {
                    p.DbType = DbType.DateTime;
                    p.Value = Convert.ToDateTime(param[i]);
                }
                else if (param[i].GetType() == typeof(bool))
                {
                    p.DbType = DbType.Boolean;
                    p.Value = Convert.ToBoolean(param[i]);
                }

                p.ParameterName = $"@{i.ToString()}";
                cmd.Parameters.Add(p);
            }

            return param;
        }


        public static object ExecRead(string sql, object[] param, DbCommand cmd)
        {
            try
            {
                cmd.CommandText = sql;
                ParamRead(param, cmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return param;
        }


        public static int ExecCount(string sql, object[] param)
        {
            int ret = 0;

            DbProviderFactory dpf = DbFactory.DbProviderFactory();
            using (DbConnection con = dpf.CreateConnection())
            {
                con.ConnectionString = DbFactory.Connection;
                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    ParamRead(param, cmd);

                    con.Open();
                    using (DbDataReader r = cmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            while (r.Read())
                            {
                                ret = Convert.ToInt32(r[0]);
                            }
                        }
                    }
                    con.Close();
                }
            }

            return ret;
        }


        public static int ExecCommand(string sql, object[] param)
        {
            int ret = 0;
            DbProviderFactory dpf = DbFactory.DbProviderFactory();

            try
            {
                using (DbConnection con = dpf.CreateConnection())
                {
                    con.ConnectionString = Connection;
                    using (DbCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        ParamRead(param, cmd);

                        con.Open();
                        ret = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return ret;
        }


        public static int ExecCommandTranx(string[] sql, object[] param)
        {
            int ret = 0;

            DbProviderFactory dpf = DbFactory.DbProviderFactory();

            try
            {
                using (DbConnection con = dpf.CreateConnection())
                {
                    con.ConnectionString = Connection;
                    con.Open();
                    using (DbCommand cmd = con.CreateCommand())
                    {
                        using (DbTransaction tranx = con.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            cmd.Transaction = tranx;
                            for (int j = 0; j < sql.Length; j++)
                            {
                                cmd.CommandText = sql[j];
                                object[] pa = param[j] as object[];
                                ParamRead(pa, cmd);
                                ret = cmd.ExecuteNonQuery();
                            }
                            tranx.Commit();
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }



        public static DbType GetDbTypeByName(string typeName)
        {
            DbType ColDbType = default(DbType);

            switch (typeName.ToUpper())
            {
                case "STRING":
                    ColDbType = DbType.String;
                    break;
                case "DECIMAL":
                    ColDbType = DbType.Decimal;
                    break;
                case "DOUBLE":
                    ColDbType = DbType.Double;
                    break;
                case "DATETIME":
                    ColDbType = DbType.DateTime;
                    break;
                case "DBNULL":
                    ColDbType = DbType.String;
                    break;
                case "INT32":
                    ColDbType = DbType.Int32;
                    break;
            }
            return ColDbType;
        }

        private static DbParameter[] ParamReadTranx(object[] strParam1, DbProviderFactory iFactory)
        {
            String sss = "";
            DbParameter p = default(DbParameter);
            int pknt = strParam1.Length;
            DbParameter[] pc = new DbParameter[pknt];
            for (int j = 0; j <= pknt - 1; j++)
            {
                if ((strParam1[j] == null))
                    strParam1[j] = "null";

                p = iFactory.CreateParameter();
                if (strParam1[j].ToString() == "-9")
                {
                    p.DbType = DbType.Int32;
                    p.Value = DBNull.Value;
                }
                else if (strParam1[j].ToString() == "01-01-01")
                {
                    p.DbType = DbType.DateTime;
                    p.Value = DBNull.Value;
                }
                else
                {
                    p.DbType = GetDbTypeByName(strParam1[j].ToString() == "null" ? "DBNULL" : strParam1[j].GetType().Name);
                    p.Value = strParam1[j].ToString() == "null" ? DBNull.Value : strParam1[j];
                }
                p.ParameterName = (j + 1).ToString();
                sss = sss + p.Value.ToString() + ",";
                pc[j] = p;

            }
            return pc;
        }


        public static object[] ParamRead2(object[] param, DbCommand[] cmd)
        {
            int len = param.Length;

            DbParameter[] p = new DbParameter[param.Length];
            for (int i = 0; i < len; i++)
            {
                p[i] = cmd[i].CreateParameter();

                if (param[i].GetType() == typeof(int))
                {
                    p[i].DbType = DbType.Int32;
                    p[i].Value = Convert.ToInt32(param[i]);
                }
                else if (param[i].GetType() == typeof(string))
                {
                    p[i].DbType = DbType.String;
                    p[i].Value = param[i].ToString();
                }
                else if (param[i].GetType() == typeof(DateTime))
                {
                    p[i].DbType = DbType.DateTime;
                    p[i].Value = Convert.ToDateTime(param[i]);
                }
                else if (param[i].GetType() == typeof(bool))
                {
                    p[i].DbType = DbType.Boolean;
                    p[i].Value = Convert.ToBoolean(param[i]);
                }

                p[i].ParameterName = $"@{i.ToString()}";
                cmd[i].Parameters.Add(p);
            }

            return param;
        }


        public static int SQLExecuteCommandTranx(string[] sql, object[] param, string cConnectString = "", string cDatabaseType = "", string cProviderName = "")
        {
            int ret = 0;
            int len = param.Length;
            DbProviderFactory dpf = DbFactory.DbProviderFactory();
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    /// DbProviderFactory iFactory = DbProviderFactories.GetFactory(cProviderName);
                    //DbProviderFactory iFactory = DBFactoryUtil.GetDbProviderFactoryFromConfigRow();
                    //StringBuilder sb = new StringBuilder();
                    using (DbConnection con = dpf.CreateConnection())
                    {
                        con.ConnectionString = Connection;

                        DbCommand[] cmd = new DbCommand[len];

                        for (Int32 i = 0; i < len; i++)
                        {
                            cmd[i] = con.CreateCommand();
                            cmd[i].CommandText = sql[i];
                            object[] p = param[i] as object[];
                            ParamRead2(p, cmd);
                            /*
                            if (param[i] != null)
                            {
                                object[] p = param[i] as object[];
                                cmd[i].Parameters.AddRange(ParamReadTranx(p, dpf));

                            }
                            */
                        }
                        con.Open();
                        try
                        {
                            for (Int32 i = 0; i < param.Length; i++)
                            {
                                if (cmd[i] != null)
                                    cmd[i].ExecuteNonQuery();
                            }
                            ts.Complete();
                            ret = 1;
                        }
                        catch (InvalidCastException)
                        {
                            ts.Dispose();
                            con.Close();
                            throw;
                        }
                        catch (Exception)
                        {
                            ts.Dispose();
                            con.Close();
                            throw;
                        }

                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return ret;
        }

    }
}
