using System.ServiceProcess;
using System.Threading;
using Serilog;

namespace CoreServiceConsoleTest
{
    public class TestService : ServiceBase
    {
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Thread _thread;

        public void StartDirect()
        {
            _thread = new Thread(() => WorkerThreadFunc(_shutdownEvent))
            {
                Name = "My Worker Thread",
                IsBackground = true
            };
            _thread.Start();
        }

        public void StopDirect()
        {
            _shutdownEvent.Set();
            if (!_thread.Join(3000))
            { // give the thread 3 seconds to stop
                _thread.Abort();
            }
        }

        protected override void OnStart(string[] args)
        {
            StartDirect();
        }

        protected override void OnStop()
        {
            StopDirect();
        }

        private static void WorkerThreadFunc(ManualResetEvent manualResetEvent)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .WriteTo.File(@"c:\temp\log.txt")
                .CreateLogger();

            var localLogger = Log.Logger.ForContext<TestService>();

            var i = 0;

            while (!manualResetEvent.WaitOne(0))
            {
                localLogger.Information($"Iterating..{i}");
                Thread.Sleep(1000);
                i += 1;
            }
        }

        
    }
}