using AspNetStandard.Diagnostics.HealthChecks.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetStandard.Diagnostics.HealthChecks.MongoDb
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private static readonly ConcurrentDictionary<string, MongoClient> _mongoClient = new ConcurrentDictionary<string, MongoClient>();
        private readonly string _dataBaseName;
        private readonly MongoClientSettings _mongoClientSettings;

        #region ctor

        public MongoDbHealthCheck(string connectionString, string dataBaseName = default)
            : this(MongoClientSettings.FromUrl(MongoUrl.Create(connectionString)), dataBaseName)
        {
            if (dataBaseName == default)
            {
                _dataBaseName = MongoUrl.Create(connectionString)?.DatabaseName;
            }
        }

        public MongoDbHealthCheck(MongoClientSettings clientSettings, string dataBaseName = default)
        {
            _dataBaseName = dataBaseName;
            _mongoClientSettings = clientSettings;
        }

        #endregion ctor

        #region Interface Implementation

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var mongoClient = _mongoClient.GetOrAdd(_mongoClientSettings.ToString(), _ => new MongoClient(_mongoClientSettings));

                if (!string.IsNullOrWhiteSpace(_dataBaseName))
                {
                    var cursor = await mongoClient.GetDatabase(_dataBaseName).ListCollectionNamesAsync(cancellationToken: cancellationToken);
                    await cursor.FirstOrDefaultAsync(cancellationToken);
                    cursor.Dispose();
                }
                else
                {
                    var cursor = await mongoClient.ListDatabaseNamesAsync(cancellationToken);
                    await cursor.FirstOrDefaultAsync(cancellationToken);
                    cursor.Dispose();
                }

                return new HealthCheckResult(HealthStatus.Healthy, "MongoDb is healthy");
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "MongoDb is unhealthy", exception: ex);
            }
        }

        #endregion Interface Implementation
    }
}