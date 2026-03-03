using System;
using System.Data;
using System.Data.SqlClient;

namespace NexusGearMS.Helpers
{
    public static class Db
    {
        private static string GetConnectionString()
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["NexusGearDB"]?.ConnectionString;

            if (string.IsNullOrEmpty(connStr))
            {
                connStr = @"Server=.;Database=NexusGearDB;Trusted_Connection=True;";
            }

            return connStr;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using (var adapter = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public static object ExecuteScalar(string sql, SqlTransaction transaction = null, params SqlParameter[] parameters)
        {
            if (transaction != null)
            {
                using (var cmd = new SqlCommand(sql, transaction.Connection, transaction))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
            else
            {
                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        public static int ExecuteNonQuery(string sql, SqlTransaction transaction = null, params SqlParameter[] parameters)
        {
            if (transaction != null)
            {
                using (var cmd = new SqlCommand(sql, transaction.Connection, transaction))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (var conn = GetConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
