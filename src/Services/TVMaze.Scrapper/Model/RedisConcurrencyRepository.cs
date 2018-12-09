namespace AUTOPAL.RTL.TVMaze.Services.TVMaze.Scrapper.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using StackExchange.Redis;

    public class RedisConcurrencyRepository : IConcurrencyRepository
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisConcurrencyRepository> _logger;

        private readonly ConnectionMultiplexer _redis;

        public RedisConcurrencyRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            this._logger = loggerFactory.CreateLogger<RedisConcurrencyRepository>();
            this._redis = redis;
            this._database = redis.GetDatabase();
        }

        public bool IsConcurrencyValueValid(KeyValuePair<string, long> pair)
        {
            var data = this._database.StringGetSet(key: pair.Key, value: pair.Value);
            if (data.IsNullOrEmpty)
            {
                return false;
            }

            return data == pair.Value;
        }

        private IServer GetServer()
        {
            var endpoint = this._redis.GetEndPoints();
            return this._redis.GetServer(endpoint: endpoint.First());
        }
    }
}