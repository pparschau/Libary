using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;


namespace Libary
{
    internal class Logger
    {
        static string ProjectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
        static string CurrentUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        static private string LogDBConnectionString = "Server = localhost ; Integrated Security = True; Initial Catalog = Logger;";

        static private bool SendEMail = true;
        static private string EMail_Betreff = ProjectName;
        static private string EMail_From = ProjectName + "_Logger@gmail.de";
        static private string EMail_To = "patrick.parschau@gmail.de";
        static private string Email_CC = "";
        static private string SMTP_Server = "smtp.gmail.com";
        static private int SMTP_Port = 25;


        public static void Log(int ErrorLevel,string Action,string Message)
        {

            //ErrorLevel

            //1=Info

            //2=Warning

            //3=Error


            LogToFile(ErrorLevel,Action,Message);

            LogToDatabase(ErrorLevel, Action, Message);

        }

        public static void GetLoggingInfo()
        {

            Console.WriteLine("Logger Info:");

        }




        public static void LogToFile(int ErrorLevel, string Action, string Message)
        {
            string LoggerPath = Directory.GetCurrentDirectory() + @"/Logger/";

            string Year = DateTime.Now.ToString("yyyy");
            string Month = DateTime.Now.ToString("MM");
            string Day = DateTime.Now.ToString("dd");
            string Hour = DateTime.Now.ToString("HH");
            string Minute = DateTime.Now.ToString("mm");
            string Second = DateTime.Now.ToString("ss");

            try {

                #region Directory and Logfile
                if (!Directory.Exists(LoggerPath))
                {
                    Directory.CreateDirectory(LoggerPath);
                }
                if (!Directory.Exists(LoggerPath + Year))
                {
                    Directory.CreateDirectory(LoggerPath + Year);
                }
                if (!Directory.Exists(LoggerPath + Year + "/" + Month))
                {
                    Directory.CreateDirectory(LoggerPath + Year + "/" + Month);
                }
                if (!Directory.Exists(LoggerPath + Year + "/" + Month + "/" + Day))
                {
                    Directory.CreateDirectory(LoggerPath + Year + "/" + Month + "/" + Day);
                }
                if(!File.Exists(LoggerPath + Year + "/" + Month + "/" + Day + "/" + ProjectName + ".txt"))
                {
                    File.Create(LoggerPath + Year + "/" + Month + "/" + Day + "/" + ProjectName + ".txt").Close();
                }
                #endregion

                string LogLine = "  " +   ErrorLevel + "  | " + Day + "." + Month + "." + Year + " " + Hour +":"+ Minute +":"+ Second + " | " + Action + " | " + Message + " | " + CurrentUserName +  Environment.NewLine;
                File.AppendAllText(LoggerPath + Year + "/" + Month + "/" + Day + "/" + ProjectName + ".txt", LogLine);
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex);
            };
        }


        public static void LogToDatabase(int ErrorLevel, string Action, string Message)
        {
            CheckTableForLogging();

            if (String.IsNullOrEmpty(Message))
            { 
                Message = "";
            }

            SqlConnection sqldbConnection = new SqlConnection(LogDBConnectionString);

            string InsertLogSQL = $@"INSERT INTO [dbo].[Logger_{ProjectName}]

            (
            [User],
            [Level],
            [Action],
            [Message],
            [Timestamp]
            )

            VALUES

            (SYSTEM_USER," + ErrorLevel + ",'" + Action + @"','" + Message.Replace(@"'", @" ") + @"',getdate()" + ")";


            DatabaseOperations.InsertOrUpdate(LogDBConnectionString, InsertLogSQL);

        }


        public static void CheckTableForLogging()
        {

            if(DatabaseOperations.CheckIfDBExists(LogDBConnectionString,"Logger",true))
            { 
            if (!DatabaseOperations.CheckIfDBTableExists(LogDBConnectionString, "Logger_" + ProjectName))
            {
                string CreateTableScript = $@"
                
                USE [{DatabaseOperations.ExtractConnectionString(LogDBConnectionString, "Initial Catalog")}]

                CREATE TABLE[dbo].[Logger_{ProjectName}] (

                   [User][nvarchar](100) NULL,
                   [Level][int] NULL,
                   [Action][text] NULL,
                   [Message][text] NULL,
                   [Timestamp][datetime] NULL);";



                  DatabaseOperations.InsertOrUpdate(LogDBConnectionString, CreateTableScript);



                  Console.WriteLine("Logging Table succesfully created at " + LogDBConnectionString + " "+ $"[dbo].[Logger_{ ProjectName}]");
                    }
                }
             
            
            }

        }
    
}
