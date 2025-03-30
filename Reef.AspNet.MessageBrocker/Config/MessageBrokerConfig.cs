namespace Reef.AspNet.MessageBrocker.Config
{
	public class MessageBrokerConfig : IMessageBrokerConfig
	{
		public string BootstrapServers { get; set; }
		public string Topic { get; set; }
		public string GroupId { get; set; }
		public string SaslUsername { get; set; }
		public string SaslPassword { get; set; }
	}
}
