using BlueBrown.Accounts.Shared.Messages;
using MassTransit;

namespace BlueBrown.Data.DataManagementPatterns.Infrastructure.Services.MessageBroker.Consumers
{
	internal class RegistrationMessageBrokerConsumer : IConsumer<Batch<UpdateCustomerMessage>>
	{
		public Task Consume(ConsumeContext<Batch<UpdateCustomerMessage>> context)
		{
			return Task.CompletedTask;
		}
	}
}
