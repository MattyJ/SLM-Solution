using System.Diagnostics;

namespace Fujitsu.SLM.CustomerMigration.Console.Core.Metrics
{
    public class PrfMon
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public PrfMon()
        {
            _stopwatch.Start();
        }

        public double Stop()
        {
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalSeconds;
        }

        public string StopString()
        {
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalSeconds.ToString("0.000");
        }
    }
}
