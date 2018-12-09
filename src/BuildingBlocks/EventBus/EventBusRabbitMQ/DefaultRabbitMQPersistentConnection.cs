namespace AUTOPAL.RTL.TVMaze.BuildingBlocks.EventBus.RabbitMQ
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;
    using global::RabbitMQ.Client.Exceptions;
    using Microsoft.Extensions.Logging;
    using Polly;

    public class DefaultRabbitMqPersistentConnection
        : IRabbitMqPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMqPersistentConnection> _logger;
        private readonly int _retryCount;

        private readonly object _syncRoot = new object();
        private IConnection _connection;
        private bool _disposed;

        public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory,
            ILogger<DefaultRabbitMqPersistentConnection> logger, int retryCount = 5)
        {
            this._connectionFactory =
                connectionFactory ?? throw new ArgumentNullException(paramName: nameof(connectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(paramName: nameof(logger));
            this._retryCount = retryCount;
        }

        public bool IsConnected
        {
            get { return this._connection != null && this._connection.IsOpen && !this._disposed; }
        }

        public IModel CreateModel()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException(
                    message: "No RabbitMQ connections are available to perform this action");
            }

            return this._connection.CreateModel();
        }

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;

            try
            {
                this._connection.Dispose();
            }
            catch (IOException ex)
            {
                this._logger.LogCritical(message: ex.ToString());
            }
        }

        public bool TryConnect()
        {
            this._logger.LogInformation(message: "RabbitMQ Client is trying to connect");

            lock (this._syncRoot)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount: this._retryCount,
                        sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromSeconds(value: Math.Pow(x: 2, y: retryAttempt)),
                        onRetry: (ex, time) => { this._logger.LogWarning(message: ex.ToString()); }
                    );

                policy.Execute(action: () =>
                {
                    this._connection = this._connectionFactory
                        .CreateConnection();
                });

                if (this.IsConnected)
                {
                    this._connection.ConnectionShutdown += this.OnConnectionShutdown;
                    this._connection.CallbackException += this.OnCallbackException;
                    this._connection.ConnectionBlocked += this.OnConnectionBlocked;

                    this._logger.LogInformation(
                        message:
                        $"RabbitMQ persistent connection acquired a connection {this._connection.Endpoint.HostName} and is subscribed to failure events");

                    return true;
                }

                this._logger.LogCritical(message: "FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (this._disposed)
            {
                return;
            }

            this._logger.LogWarning(message: "A RabbitMQ connection is shutdown. Trying to re-connect...");

            this.TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (this._disposed)
            {
                return;
            }

            this._logger.LogWarning(message: "A RabbitMQ connection throw exception. Trying to re-connect...");

            this.TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (this._disposed)
            {
                return;
            }

            this._logger.LogWarning(message: "A RabbitMQ connection is on shutdown. Trying to re-connect...");

            this.TryConnect();
        }
    }
}