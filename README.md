# Reef.AspNet.MessageBroker

**Reef.AspNet.MessageBroker** is a library that simplifies integration with MassTransit Kafka in ASP.NET.

## 📌 Features
- Abstracts MassTransit Kafka configuration
- Supports SASL authentication
- Simplified consumer registration
- Automatic message type-to-topic mapping

## 🚀 Installation
Since the library is not available on NuGet, you need to add it manually.

1. Copy `Reef.AspNet.MessageBroker.dll` into your project.
2. Add a reference to the DLL in your project.

## ⚙️ Usage

### 1. Define a Message
Messages should implement `IMessage`:

```csharp
using Reef.AspNet.MessageBrocker.Contracts;

public class MyMessage : IMessage
{
    public string Text { get; set; } = string.Empty;
}
```

### 2. Create a Consumer

```csharp
using MassTransit;
using Reef.AspNet.MessageBrocker.Consumers;

public class MyMessageConsumer : IMessageConsumer<MyMessage>
{
    public async Task Consume(ConsumeContext<MyMessage> context)
    {
        Console.WriteLine($"Received: {context.Message.Text}");
        await Task.CompletedTask;
    }
}
```

### 3. Configure Message Broker in `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

var kafkaConfig = builder.Configuration.GetSection("KafkaConfig").Get<MessageBrokerConfig>()
    ?? throw new InvalidOperationException("Kafka configuration is missing");

builder.AddMessageBroker(kafkaConfig, x => x.AddConsumer<MyMessageConsumer>(),
    new Dictionary<Type, string> { { typeof(MyMessage), "my-topic" } });

var app = builder.Build();
app.Run();
```

### 4. Example `appsettings.json` Configuration

```json
{
  "KafkaConfig": {
    "BootstrapServers": "localhost:9092",
    "Topic": "my-topic",
    "GroupId": "my-group",
    "SaslUsername": "",
    "SaslPassword": ""
  }
}
```

### 5. Publishing Messages
You can publish messages using `IPublishEndpoint`:

```csharp
public class MyService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MyService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task SendMessageAsync()
    {
        await _publishEndpoint.Publish(new MyMessage { Text = "Hello Kafka!" });
    }
}
```

## 🛠 Requirements
- .NET 8+
- Kafka 2.x+
- MassTransit 8+

## 📜 License
This project is licensed under the Apache License 2.0 License.

## 📧 Contact
If you have any questions or suggestions, create an issue or contact me via GitHub.
