using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libary
{
    internal class DatabaseOperations
    {

        public static string ExtractConnectionString(string ConnectionString, string Object)
        {
            //Extract an object value of a Connection String and trim spaces and start and end
            //Example ConnectionString Format = " Server = ServerName ; Integrated Security = True; Initial Catalog = DB_Main;"


            int PositionObject = ConnectionString.IndexOf(Object) + Object.Length;
            int EqualsPosition = ConnectionString.Substring(PositionObject).IndexOf("=") + 1;

            string TrimmedConnectionString = ConnectionString.Substring(PositionObject + EqualsPosition);

            string ObjectResult = TrimmedConnectionString.Substring(0, TrimmedConnectionString.IndexOf(";")).Trim();

            return ObjectResult;
        }

        public static bool CheckIfDBTableExists(string ConnectionString, string TableName)
        {
            bool Exists = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {

                String TestQuery = $"SELECT 1 FROM [{DatabaseOperations.ExtractConnectionString(ConnectionString, "Initial Catalog")}].[dbo].[{TableName}]";

                try
                {


                    using (SqlCommand command = new SqlCommand(TestQuery, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Exists = true;
                            }
                        }

                    }
                }

                catch(Exception)
                {
                    Exists = false;
                }
            }

            return Exists;

        }

    }
}
