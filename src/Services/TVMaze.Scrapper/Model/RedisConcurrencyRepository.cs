using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace AUTOPOAL.RTL.TVMaze.Services.TVMaze.Scrapper.Model
{
    public class RedisConcurrencyRepository : IConcurrencyRepository
    {
        private readonly ILogger<RedisConcurrencyRepository> _logger;

        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisConcurrencyRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
            _logger = loggerFactory.CreateLogger<RedisConcurrencyRepository>();
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public bool IsConcurrencyValueValid(KeyValuePair<string, long> pair)
        {
            RedisValue data = _database.StringGetSet(pair.Key, pair.Value);
            if (data.IsNullOrEmpty)
            {
                return false;
            }

            return data == pair.Value;
        }

        private IServer GetServer()
        {
            System.Net.EndPoint[] endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }
    }
}