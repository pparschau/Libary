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
                    Logger.Log(3, "CheckIfDBExists", ex.ToString());
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

        public static List<string> DB2List(string ConnectionString, string SQL_Statement)
        {
            //When File Exists, create Another one?
            //Datentypen herausfinden


            // nach dem ersten Select ein Top1
            bool IsHeader=true;
            string Temp_line="";
            List<string> Result = new List<string>();

            SqlConnection connection = new SqlConnection (ConnectionString);
            connection.Open();

            SqlCommand selectCommand = new SqlCommand(SQL_Statement,connection);

            SqlDataReader sqlreader = null;
            sqlreader = selectCommand.ExecuteReader();

            while(sqlreader.Read())
            {
                if(IsHeader)
                {
                //Build Header

                for(int i = 0; i < sqlreader.FieldCount; i++)
                {
                    Temp_line+=(sqlreader.GetName(i))+ ";";

                }
                Result.Add(Temp_line);
                Console.WriteLine(Temp_line);
                Temp_line = String.Empty;
                IsHeader=false;
                //End Build Header
                }


                for (int i=0; i< sqlreader.FieldCount; i++)
                {
                    if (sqlreader[i].GetType().ToString()=="System.DateTime")
                    {
                    Temp_line += Convert.ToDateTime(sqlreader[i]).ToString("yyyy-MM-dd HH:mm:ss") + ";";
                    }

                    else if (sqlreader[i].GetType().ToString()=="System.Double")
                    {
                    Temp_line += Convert.ToDateTime(sqlreader[i]).ToString().Replace(".",",") + ";";
                    }

                    else
                    {
                    Temp_line += (sqlreader[i]).ToString().Replace("\r",Environment.NewLine) + ";";
                    }
                }
                Console.WriteLine(Temp_line);
                Result.Add(Temp_line);
                Temp_line = String.Empty;
            }

            return Result;
        


        }

        public static void CSV2DB(string Path,string ConnectionString, string TableName, bool ClearTableBeforeInsert)
        {


        }

          public static void DB2CwSV(string ConnectionStringSource, string TableSource, string ConnectionStringTarget, string TableTarget)
        {
            
           

        }

        public static void GenerateCreateTableScript(string ConnectionString, string TableName)
        {


            string GenerateScript = $@"
            DECLARE
                  @object_name SYSNAME
                , @object_id INT
                , @SQL NVARCHAR(MAX)
            SELECT
                  @object_name = '[' + OBJECT_SCHEMA_NAME(o.[object_id]) + '].[' + OBJECT_NAME([object_id]) + ']'
                , @object_id = [object_id]
            FROM (SELECT [object_id] = OBJECT_ID('dbo.{TableName}', 'U')) o
            SELECT @SQL = 'CREATE TABLE ' + @object_name + CHAR(13) + '(' + CHAR(13) + STUFF((
                SELECT CHAR(13) + '    , [' + c.name + '] ' + 
                    CASE WHEN c.is_computed = 1
                        THEN 'AS ' + OBJECT_DEFINITION(c.[object_id], c.column_id)
                        ELSE 
                            CASE WHEN c.system_type_id != c.user_type_id 
                                THEN '[' + SCHEMA_NAME(tp.[schema_id]) + '].[' + tp.name + ']' 
                                ELSE '[' + UPPER(tp.name) + ']' 
                            END  + 
                            CASE 
                                WHEN tp.name IN ('varchar', 'char', 'varbinary', 'binary')
                                    THEN '(' + CASE WHEN c.max_length = -1 
                                                    THEN 'MAX' 
                                                    ELSE CAST(c.max_length AS VARCHAR(5)) 
                                                END + ')'
                                WHEN tp.name IN ('nvarchar', 'nchar')
                                    THEN '(' + CASE WHEN c.max_length = -1 
                                                    THEN 'MAX' 
                                                    ELSE CAST(c.max_length / 2 AS VARCHAR(5)) 
                                                END + ')'
                                WHEN tp.name IN ('datetime2', 'time2', 'datetimeoffset') 
                                    THEN '(' + CAST(c.scale AS VARCHAR(5)) + ')'
                                WHEN tp.name = 'decimal'
                                    THEN '(' + CAST(c.[precision] AS VARCHAR(5)) + ',' + CAST(c.scale AS VARCHAR(5)) + ')'
                                ELSE ''
                            END +
                            CASE WHEN c.collation_name IS NOT NULL AND c.system_type_id = c.user_type_id 
                                THEN ' COLLATE ' + c.collation_name
                                ELSE ''
                            END +
                            CASE WHEN c.is_nullable = 1 
                                THEN ' NULL'
                                ELSE ' NOT NULL'
                            END +
                            CASE WHEN c.default_object_id != 0 
                                THEN ' CONSTRAINT [' + OBJECT_NAME(c.default_object_id) + ']' + 
						             ' DEFAULT ' + OBJECT_DEFINITION(c.default_object_id)
                                ELSE ''
                            END + 
				            CASE WHEN cc.[object_id] IS NOT NULL 
					            THEN ' CONSTRAINT [' + cc.name + '] CHECK ' + cc.[definition]
					            ELSE ''
				            END	+
                            CASE WHEN c.is_identity = 1 
                                THEN ' IDENTITY(' + CAST(IDENTITYPROPERTY(c.[object_id], 'SeedValue') AS VARCHAR(5)) + ',' + 
                                                CAST(IDENTITYPROPERTY(c.[object_id], 'IncrementValue') AS VARCHAR(5)) + ')' 
                                ELSE '' 
                            END 
                    END
                FROM sys.columns c WITH(NOLOCK)
                JOIN sys.types tp WITH(NOLOCK) ON c.user_type_id = tp.user_type_id
	            LEFT JOIN sys.check_constraints cc WITH(NOLOCK) ON c.[object_id] = cc.parent_object_id AND cc.parent_column_id = c.column_id
                WHERE c.[object_id] = @object_id
                ORDER BY c.column_id
                FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 7, '      ') + 
                ISNULL((SELECT '
                , CONSTRAINT [' + i.name + '] PRIMARY KEY ' + 
	            CASE WHEN i.index_id = 1 
		            THEN 'CLUSTERED' 
		            ELSE 'NONCLUSTERED' 
	            END +' (' + (
                SELECT STUFF(CAST((
                    SELECT ', [' + COL_NAME(ic.[object_id], ic.column_id) + ']' +
                            CASE WHEN ic.is_descending_key = 1
                                THEN ' DESC'
                                ELSE ''
                            END
                    FROM sys.index_columns ic WITH(NOLOCK)
                    WHERE i.[object_id] = ic.[object_id]
                        AND i.index_id = ic.index_id
                    FOR XML PATH(N''), TYPE) AS NVARCHAR(MAX)), 1, 2, '')) + ')'
                FROM sys.indexes i WITH(NOLOCK)
                WHERE i.[object_id] = @object_id
                    AND i.is_primary_key = 1), '') + CHAR(13) + ');'
	            select top 1 @SQL as CreateStatement";

            string Create = DB2List(ConnectionString, GenerateScript)[1];
          
            Console.WriteLine(@Create);



        }

        public static void Select(string ConnectionString, string SQLStatement)
        {
         

            


        }
        
    }
}
