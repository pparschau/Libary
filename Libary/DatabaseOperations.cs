using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libary
{
    internal class DatabaseOperations
    {

        public static string ExtractConnectionString(string ConnectionString, string Object)
        {
            //Example ConnectionString Format = " Server = ServerName ; Integrated Security = True; Initial Catalog = DB_Main;"


            int PositionObject = ConnectionString.IndexOf(Object) + Object.Length;
            int EqualsPosition = ConnectionString.Substring(PositionObject).IndexOf("=") + 1;

            string TrimmedConnectionString = ConnectionString.Substring(PositionObject + EqualsPosition);

            string ObjectResult = TrimmedConnectionString.Substring(0, TrimmedConnectionString.IndexOf(";")).Trim();

            return ObjectResult;
        }

    }
}
