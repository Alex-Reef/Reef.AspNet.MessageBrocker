using MassTransit;
using Reef.AspNet.MessageBrocker.Contracts;

namespace Reef.AspNet.MessageBrocker.Consumers
{
	public interface IMessageConsumer<TMessage> : IConsumer<TMessage>
		where TMessage : class, IMessage
	{
	}
}
