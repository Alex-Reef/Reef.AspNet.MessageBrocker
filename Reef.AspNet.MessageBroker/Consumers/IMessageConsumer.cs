using MassTransit;
using Reef.AspNet.MessageBroker.Contracts;

namespace Reef.AspNet.MessageBroker.Consumers
{
	public interface IMessageConsumer<TMessage> : IConsumer<TMessage>
		where TMessage : class, IMessage
	{
	}
}
