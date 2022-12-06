using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Libary
{
    internal class Async
    {

        static int MaxNumberofRuns = 50;

        //Call function :  Task.WhenAll(Async.ControlAsync(list));
        public static async Task ControlAsync(List<string> Liste)
        {

            List<Task> tasks = new List<Task>();

            foreach(string s in Liste)
            {

                tasks.Add(Task.Factory.StartNew(()=>DoSomeThing(s,0)));

            }

            await Task.WhenAll(tasks);

        }

        public static void DoSomeThing(string S,int AsyncRun)
        {


            try {
                Console.WriteLine("Id: "+ Task.CurrentId + "wait "+S);
            Thread.Sleep(Convert.ToInt32(S));//Simulate work
            }
            catch(Exception Ex)
            {
                if (AsyncRun> MaxNumberofRuns)
                {
                    return;
                }
                Console.WriteLine("Error: "+ S + Ex.Message);
                Thread.Sleep(AsyncRun * 10000);
                DoSomeThing(S, AsyncRun + 1);
            }








        }

    }
}
