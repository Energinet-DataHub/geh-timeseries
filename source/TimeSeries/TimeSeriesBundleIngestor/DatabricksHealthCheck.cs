using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.TimeSeries.MessageReceiver;

public class DatabricksHealthCheck : IHealthCheck
{
    private const string JobName = "PublisherStreamingJob";
    private readonly DatabricksClient _client;
    private readonly ILogger<DatabricksHealthCheck> _logger;

    public DatabricksHealthCheck(DatabricksClient client, ILogger<DatabricksHealthCheck> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetClustersAsync();

        if (response?.Clusters == null)
            return await Task.FromResult(HealthCheckResult.Unhealthy("Was not able to get a successfully response from Databricks API"));

        var runningJobs = response.Clusters.Where(x => x.State == "RUNNING" && x.DefaultTags?.RunName == JobName).ToList();

        var job = runningJobs.FirstOrDefault();
        if (job != null)
        {
            return await Task.FromResult(HealthCheckResult.Healthy($@"{JobName} with JobId {job.DefaultTags?.JobId} is running on Cluster {job.DefaultTags?.ClusterName}"));
        }
        else
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy("No running job named {JobName}"));
        }
    }
}
