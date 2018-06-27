using System;
using System.ServiceProcess;
using System.Threading;

namespace CoreServiceConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var runAsService = !Environment.UserInteractive;

            if (runAsService)
            {
                var testService = new TestService();

                ServiceBase.Run(testService);
            }
            else
            {
                var testService = new TestService();
                testService.StartDirect();

                Console.WriteLine("Press a key to exit...");
                Console.ReadKey();
                testService.StopDirect();
            }
        }
    }
}
