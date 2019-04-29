using NServiceBus;
using Messages;
using System.Threading.Tasks;
using NServiceBus.Logging;

public class OrderPlacedHandler : IHandleMessages<OrderPlaced>
{
    private static ILog log = LogManager.GetLogger<OrderPlacedHandler>();

    public Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        log.Info($"Received OrderPlaced, OrderId = {message.OrderId} - Charging credit card...");

        return context.Publish(new OrderBilled { OrderId = message.OrderId});
    }
}