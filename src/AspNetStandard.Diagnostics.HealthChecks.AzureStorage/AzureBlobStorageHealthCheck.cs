using AspNetStandard.Diagnostics.HealthChecks.Entities;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using System;

namespace AspNetStandard.Diagnostics.HealthChecks.AzureStorage
{
    public sealed class AzureBlobStorageHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureBlobStorageHealthCheck(string connectionString, string containerName = default)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _containerName = containerName;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(_connectionString);
                var serviceProperties = await blobServiceClient.GetPropertiesAsync(cancellationToken);

                if (!string.IsNullOrWhiteSpace(_containerName))
                {
                    var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

                    if (!await containerClient.ExistsAsync(cancellationToken))
                        return new HealthCheckResult(HealthStatus.Unhealthy, $"Container '{_containerName}' not exists");

                    await containerClient.GetPropertiesAsync(cancellationToken: cancellationToken);
                }

                return new HealthCheckResult(HealthStatus.Healthy, "AzureBlobStorage is healthy");
            }
            catch (Exception exception)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, "AzureBlobStorage is unhealthy", exception);
            }
        }
    }
}