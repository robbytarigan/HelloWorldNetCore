using System.Threading.Tasks;
using NServiceBus;
using Messages;
using NServiceBus.Logging;

public class OrderBilledHandler : IHandleMessages<OrderBilled>
{
    private static ILog log = LogManager.GetLogger<OrderBilledHandler>();
    public Task Handle(OrderBilled message, IMessageHandlerContext context)
    {
        log.Info($"Shipping.OrderBilledHandler Received OrderBilled, OrderId = {message.OrderId} - Should we ship now?");

        return Task.CompletedTask;
    }
}