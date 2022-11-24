using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Libary
{
    internal class DatabaseOperations
    {



        public static bool InsertOrUpdate(string connectionctring, string command)
        {
            bool success = false;

            using (var connection = new SqlConnection(connectionctring))
            {

                try
                {

                    connection.Open();
                    using(var sqlCommand = new SqlCommand(command, connection))
                    {
                        sqlCommand.CommandTimeout = 1000;
                        sqlCommand.ExecuteNonQuery();
                        success = true;
                    }


                }
                catch(Exception Ex)
                { 
                    Logger.Log(3, "InsertOrUpdate: " + command,Ex.ToString());

                }
                finally
                {
                    connection.Close();
                }


            }
            return success;

        }


        public static string ExtractConnectionString(string ConnectionString, string Object)
        {
            //Extract an object value of a Connection String and trim spaces and start and end
            //Example ConnectionString Format = " Server = ServerName ; Integrated Security = True; Initial Catalog = DB_Main;"


            string ObjectResult = "";
            try
            {
            int PositionObject = ConnectionString.IndexOf(Object) + Object.Length;
            int EqualsPosition = ConnectionString.Substring(PositionObject).IndexOf("=") + 1;

            string TrimmedConnectionString = ConnectionString.Substring(PositionObject + EqualsPosition);

            ObjectResult = TrimmedConnectionString.Substring(0, TrimmedConnectionString.IndexOf(";")).Trim();
            }

            catch(Exception Ex)
            {
                Logger.Log(3, "ExtractConnectionString", Ex.ToString());
            }
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
                                
                            }
                            Exists = true;
                        }

                    }
                }

                catch(Exception Ex)
                {
                    Logger.Log(3, "CheckIfDBTableExists", Ex.ToString());

                    Exists = false;
                }
                finally
                {
                    connection.Close();
                }

            }
            

            return Exists;

        }




        public static bool CheckIfDBExists(string ConnectionString, string DatabaseName,bool CreateItIfNotExsists)
        {
            bool Exists = false;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {

                String TestQuery = $"use {DatabaseName};";

                try
                {
                    using (SqlCommand command = new SqlCommand(TestQuery, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                               
                            }
                            Exists = true;
                        }

                    }
                }

                catch (Exception ex)
                {
                    Exists = false;
                    Logger.Log(3, "CheckIfDBExists", Ex.ToString());
                }
                finally
                {
                    connection.Close();
                }
            }

            if ((CreateItIfNotExsists) && (!Exists))
            {
                Exists = (CreateDatabase(ConnectionString, $"CREATE DATABASE [{DatabaseName}];"));
            }

            return Exists;

        }
        public static bool CreateDatabase(string connectionctring,string command)
        {

            string ModConnectionString = "Server = " + ExtractConnectionString(connectionctring, "Server") + ";Integrated Security = True;";
            //Exclude the DB Name

            bool status = InsertOrUpdate(ModConnectionString, command);

            Thread.Sleep(10000);
            //Give DB System Time to create DB safe
            return status;

        }

    }
}
