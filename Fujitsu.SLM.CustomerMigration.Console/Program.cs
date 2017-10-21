using Fujitsu.SLM.CustomerMigration.Console.Commands;
using Fujitsu.SLM.CustomerMigration.Console.Core.Metrics;

namespace Fujitsu.SLM.CustomerMigration.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine(@"Fujitsu.SLM.CustomerMigration.Console.cs -> Main. Started.");
            var customerMigrationMainPrfMon = new PrfMon();

            var copyFunction = new CopyFunction();
            copyFunction.Execute();

            System.Console.WriteLine(@"Fujitsu.SLM.CustomerMigration.Console.cs -> Main Completed. Average Execution: {0:0.000}s", customerMigrationMainPrfMon.Stop());
            System.Console.ReadLine();
        }
    }
}
