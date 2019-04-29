using System.Threading.Tasks;
using NServiceBus;
using Messages;
using NServiceBus.Logging;

public class OrderPlacedHandler : IHandleMessages<OrderPlaced>
{
    private static ILog log = LogManager.GetLogger<OrderPlacedHandler>();

    public Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        log.Info($"Shipping.OrderPlacedHandler Received OrderPlaced, OrderId = {message.OrderId} - Should we ship now?");

        return Task.CompletedTask;
    }
}