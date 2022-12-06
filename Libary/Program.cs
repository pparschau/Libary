using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Libary
{
    internal class Program
    {
        static void Main(string[] args)
        {

          //  Logger.Log(1,"TestAction","Erro at...");

   //         Console.WriteLine(WindowsServices.GetServiceStatus(null, "MSSQLSERVER1"));


            List<string> list = new List<string>();
            list.Add("5000");
            list.Add("1000");
            list.Add("1000");

           Task.WhenAll(Async.ControlAsync(list));

            Console.Read();
        }
    }
}
