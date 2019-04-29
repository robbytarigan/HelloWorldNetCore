using System;
using System.Threading.Tasks;
using NServiceBus;

namespace Sales
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Sales";

            var endpointConfiguration = new EndpointConfiguration("Sales");

            var transport = endpointConfiguration.UseTransport<LearningTransport>();

            #region NoDelayedEntries
            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Delayed(delayed => delayed.NumberOfRetries(0));
            #endregion

            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");

            Console.ReadLine();

            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
