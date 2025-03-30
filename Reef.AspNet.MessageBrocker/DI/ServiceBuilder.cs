using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Reef.AspNet.MessageBroker.Config;

namespace Reef.AspNet.MessageBroker.DI
{
	public static partial class ServiceBuilder
	{
		public static WebApplicationBuilder AddMessageBroker<TConfig>(
			this WebApplicationBuilder builder,
			TConfig config,
			Action<IBusRegistrationConfigurator> configureConsumers,
			Dictionary<Type, string> topicMappings)
			where TConfig : class, IMessageBrokerConfig
		{
			builder.Services.AddMassTransit(x =>
			{
				configureConsumers(x);

				x.AddRider(rider =>
				{
					rider.UsingKafka((context, k) =>
					{
						k.Host(config.BootstrapServers, h =>
						{
							if (!string.IsNullOrEmpty(config.SaslUsername))
							{
								h.UseSasl(s =>
								{
									s.Username = config.SaslUsername;
									s.Password = config.SaslPassword;
									s.Mechanism = Confluent.Kafka.SaslMechanism.Plain;
								});
							}
						});

						foreach (var mapping in topicMappings)
						{
							var messageType = mapping.Key;
							var topicName = mapping.Value;

							var consumerType = typeof(IConsumer<>).MakeGenericType(messageType);
							var consumerRegistration = x.GetType()
								.GetMethod(nameof(x.AddConsumer), Type.EmptyTypes)
								?.MakeGenericMethod(messageType);

							consumerRegistration?.Invoke(x, null);

							var topicEndpointMethod = typeof(IKafkaFactoryConfigurator)
								.GetMethod(nameof(k.TopicEndpoint))
								?.MakeGenericMethod(messageType);

							topicEndpointMethod?.Invoke(k, new object[]
							{
								topicName,
								config.GroupId,
								new Action<IKafkaTopicReceiveEndpointConfigurator>(e =>
								{
									e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
								})
							});
						}
					});
				});
			});

			builder.Services.AddScoped<IPublishEndpoint>(provider => provider.GetRequiredService<IBus>());

			return builder;
		}
	}
}
