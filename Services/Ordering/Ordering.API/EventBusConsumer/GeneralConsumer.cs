using EventBus.Messages.Events;
using MassTransit;

namespace Ordering.API.EventBusConsumer
{
    public class GeneralConsumer : IConsumer<IntegrationBaseEvent>
    {
        private readonly ILogger<GeneralConsumer> _logger;

        public GeneralConsumer(ILogger<GeneralConsumer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<IntegrationBaseEvent> context)
        {
            _logger.LogInformation("IntegrationBaseEvent consumed successfully. ConversationId : {ConversationId}", context.ConversationId);
        }
    }
}