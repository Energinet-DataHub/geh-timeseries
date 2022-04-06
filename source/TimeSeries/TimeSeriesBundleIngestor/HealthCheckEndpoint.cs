﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energinet.DataHub.Core.App.FunctionApp.Diagnostics.HealthChecks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Energinet.DataHub.TimeSeries.MessageReceiver
{
    public class HealthCheckEndpoint
    {
        public HealthCheckEndpoint(IHealthCheckEndpointHandler healthCheckEndpointHandler)
        {
            EndpointHandler = healthCheckEndpointHandler;
        }

        private IHealthCheckEndpointHandler EndpointHandler { get; }

        [Function("HealthCheck")]
        public Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "monitor/{endpoint}")]
            HttpRequestData httpRequest,
            string endpoint)
        {
            return EndpointHandler.HandleAsync(httpRequest, endpoint);
        }
    }
}