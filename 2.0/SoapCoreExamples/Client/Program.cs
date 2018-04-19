using System;
using System.ServiceModel;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(new Uri("http://localhost:5000/PingService.svc"));
            var channelFactory = new ChannelFactory<IPingService>(binding, endpoint);
            var serviceClient = channelFactory.CreateChannel();
            var result = serviceClient.Ping("Ping");
            channelFactory.Close();
            Console.WriteLine("Receive result: {0}", result);
        }
    }
}
