using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Libary
{
    internal class WindowsServices
    {

        public static void RestartService(string Target,string ServiceName)
        {
            try
            {
                ServiceController sc = new ServiceController(ServiceName, Target);

            if (sc.Status.Equals(ServiceControllerStatus.Running))
            {

                StopService(Target, ServiceName);
                StartService(Target, ServiceName);
            }

            if (sc.Status.Equals(ServiceControllerStatus.Stopped))
            {
                StartService(Target, ServiceName);
                StopService(Target, ServiceName);
                StartService(Target, ServiceName);
            }
            }
            catch (Exception Ex)
            {
                Logger.Log(2, "RestartService", Ex.ToString());
            }

}
        public static void StartService(string Target, string ServiceName)
        {
            try
            {
                ServiceController sc = new ServiceController(ServiceName, Target);
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.StartPending);

            }
            catch (Exception Ex)
            {
                Logger.Log(2, "StartService", Ex.ToString());
            }

        }

        public static void StopService(string Target, string ServiceName)
        {
            try
            {
            ServiceController sc = new ServiceController(ServiceName, Target);
            sc.Stop();
            sc.WaitForStatus(ServiceControllerStatus.Stopped);
            
            }
            catch (Exception Ex)
            {
                Logger.Log(2, "StopService", Ex.ToString());
            }

}

        public static string GetServiceStatus(string Target, string ServiceName)
        {

            string Status=  "n.A.";

            try { 

            if (String.IsNullOrEmpty(Target))
            { 
                ServiceController sc = new ServiceController(ServiceName);
                Status = sc.Status.ToString();
            }
            else 
            {
            ServiceController sc = new ServiceController(ServiceName, Target);
                Status = sc.Status.ToString();
            }
            }
            catch(Exception Ex)
            {
                Status = "n.A.";
                Logger.Log(2, "GetServiceStatus", Ex.ToString());
            }


            return Status;

        }


    }
}
