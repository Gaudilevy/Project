using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Project
{
      public static class Database
      {
            public static string connectionString;

            public static void PullFromDatabase(string query, Action<SqlCommand> configureCommand, Action<SqlDataReader> doWithEachRow)
            {
                  using (SqlConnection conn = new SqlConnection(connectionString))
                  {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
					configureCommand?.Invoke(cmd);

					using (SqlDataReader dr = cmd.ExecuteReader())
                              {
                                    while (dr.Read())
                                    {
                                          doWithEachRow(dr);
                                    }
                              }
                        }
                  }
            }

            public static int ExecuteCommand(string query, Action<SqlCommand> configureCommand,bool shouldExecuteScalar = false)
            {
                  int result = 0;
                  using (SqlConnection conn = new SqlConnection(connectionString))
                  {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
					configureCommand?.Invoke(cmd);

					result = shouldExecuteScalar ? Convert.ToInt32(cmd.ExecuteScalar()): cmd.ExecuteNonQuery();
                        }
                  }

                  return result;
            }

            // Extention method for SqlDataReader to check for nulls returned fron database
            public static string GetStringOrNull(this SqlDataReader dr, int i)
            {
                  if (dr.IsDBNull(i))
                  {
                        return null;
                  }
                  return dr.GetString(i);
            }

		// Extention method for SqlParameterCollection to check for C# nulls sent to database
		public static void AddWithValueCheckNull(this SqlParameterCollection parameters, string parameterName, object value)
            {
                  parameters.AddWithValue(parameterName, value == null ? DBNull.Value : value);
            }

      }
}
