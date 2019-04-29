
using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace Sales {
    public class PlaceOrderHandler : IHandleMessages<PlaceOrder>
    {
        private static ILog log = LogManager.GetLogger<PlaceOrderHandler>();
        private static Random random = new Random();
        
        public Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            log.Info($"Received PlaceOrder, OrderId = {message.OrderId}");

            // This is where normally some business logic would occur
            // Transient error
            // if (random.Next(0, 5) == 0) {
            //     throw new Exception("Oops");
            // }

            // Throwing a systematic exception
            // throw new Exception("BOOM");
            
            var orderPlaced = new OrderPlaced { OrderId = message.OrderId };

            return context.Publish(orderPlaced);
        }
    }
}