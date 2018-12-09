namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.RabbitMQ
{
    using System;
    using global::RabbitMQ.Client;

    public interface IRabbitMqPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}