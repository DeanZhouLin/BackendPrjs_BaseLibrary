using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Com.BaseLibrary.Utility;
using System.Data.SqlClient;

namespace Com.BaseLibrary.Data
{
	//public static class DBHelper
	//{
	//    static string connectionString = ConfigurationHelper.GetConnectionString("AppDataConnectionString");
	//    public static DataTable GetTable( string sql)
	//    {
            
	//        using (SqlConnection conn = new SqlConnection(connectionString))
	//        {
	//            SqlCommand command = new SqlCommand(sql, conn);
	//            SqlDataAdapter da = new SqlDataAdapter(command);
	//            DataSet ds = new DataSet("dt");
	//            da.Fill(ds);
	//            DataTable dt = ds.Tables[0];
	//            return dt;
	//        }
	//    }

	//    public static void ExecuteNonQuery(string commandText, Dictionary<string, object> paramters)
	//    {
	//        using (SqlConnection conn = new SqlConnection(connectionString))
	//        {
	//            SqlCommand command = new SqlCommand(commandText, conn);
	//            foreach (var item in paramters)
	//            {
	//                command.Parameters.AddWithValue(item.Key, item.Value);
	//            }
	//            conn.Open();
	//            command.ExecuteNonQuery();
	//            conn.Close();
	//        }
	//    }
	//    public static T ExecuteScalar<T>(string commandText, Dictionary<string, object> paramters)
	//    {
	//        return ExecuteScalar<T>(commandText, paramters, CommandType.Text);
	//    }

	//    public static T ExecuteScalar<T>(string commandText, Dictionary<string, object> paramters, CommandType commandType)
	//    {
	//        using (SqlConnection conn = new SqlConnection(connectionString))
	//        {
	//            SqlCommand command = new SqlCommand(commandText, conn);
	//            command.CommandType = commandType;
	//            foreach (var item in paramters)
	//            {
	//                command.Parameters.AddWithValue(item.Key, item.Value);
	//            }
	//            conn.Open();
	//            object t = command.ExecuteScalar();
	//            if (t == null)
	//            {
	//                return default(T);
	//            }
	//            return StringUtil.ConvertToT<T>(t.ToString());
	//            // conn.Close();
	//        }
	//    }
	//}
}
