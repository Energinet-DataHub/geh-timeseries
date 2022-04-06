﻿// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;

using Energinet.DataHub.Core.App.Common.Diagnostics.HealthChecks;

using Energinet.DataHub.Core.App.FunctionApp.Diagnostics.HealthChecks;
using Energinet.DataHub.Core.App.FunctionApp.Middleware;
using Energinet.DataHub.Core.App.FunctionApp.Middleware.CorrelationId;
using Energinet.DataHub.Core.JsonSerialization;
using Energinet.DataHub.Core.Logging.RequestResponseMiddleware;
using Energinet.DataHub.Core.Logging.RequestResponseMiddleware.Storage;
using Energinet.DataHub.TimeSeries.Application;
using Energinet.DataHub.TimeSeries.Application.CimDeserialization.TimeSeriesBundle;
using Energinet.DataHub.TimeSeries.Infrastructure.Authentication;
using Energinet.DataHub.TimeSeries.Infrastructure.EventHub;
using Energinet.DataHub.TimeSeries.Infrastructure.Functions;
using Energinet.DataHub.TimeSeries.Infrastructure.Registration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.TimeSeries.MessageReceiver
{
    public abstract class Startup
    {
        protected Startup()
        {
        }

#pragma warning disable CA1822 // Mark members as static
        protected async Task ExecuteApplicationAsync(IHost host)
#pragma warning restore CA1822 // Mark members as static
        {
            await host.RunAsync().ConfigureAwait(false);
        }

        protected IHost ConfigureApplication()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(ConfigureFunctionsWorkerDefaults)
                .ConfigureServices(ConfigureServices)
                .Build();
            return host;
        }

        protected virtual void ConfigureFunctionsWorkerDefaults(IFunctionsWorkerApplicationBuilder options)
        {
            options.UseMiddleware<RequestResponseLoggingMiddleware>();
            options.UseMiddleware<JwtTokenWrapperMiddleware>();
            options.UseMiddleware<CorrelationIdMiddleware>();
            options.UseMiddleware<FunctionTelemetryScopeMiddleware>();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddApplicationInsightsTelemetryWorkerService(
                Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY"));

            serviceCollection.AddLogging();
            serviceCollection.AddScoped<ICorrelationContext, CorrelationContext>();
            serviceCollection.AddScoped<CorrelationIdMiddleware>();
            serviceCollection.AddScoped<FunctionTelemetryScopeMiddleware>();
            serviceCollection.AddScoped<IHttpResponseBuilder, HttpResponseBuilder>();
            serviceCollection.AddScoped<TimeSeriesBundleDtoValidatingDeserializer>();
            serviceCollection.AddScoped<ITimeSeriesForwarder, TimeSeriesForwarder>();
            serviceCollection.AddScoped<IEventDataFactory, EventDataFactory>();
            serviceCollection.AddScoped<TimeSeriesBundleIngestorEndpoint>();
            serviceCollection
                .AddScoped<ITimeSeriesBundleDtoValidatingDeserializer, TimeSeriesBundleDtoValidatingDeserializer>();
            serviceCollection.AddSingleton<IJsonSerializer, JsonSerializer>();
            serviceCollection.AddScoped<IEventHubSender>(provider => new EventHubSender(
                EnvironmentHelper.GetEnv("EVENT_HUB_CONNECTION_STRING"),
                EnvironmentHelper.GetEnv("EVENT_HUB_NAME"),
                provider.GetService<IEventDataFactory>() !));

            serviceCollection.AddScoped<ITimeSeriesForwarder, TimeSeriesForwarder>();

            serviceCollection.AddJwtTokenSecurity();

            serviceCollection.AddSingleton<IRequestResponseLogging>(provider =>
            {
                var logger = provider.GetService<ILogger<RequestResponseLoggingBlobStorage>>();
                var storage = new RequestResponseLoggingBlobStorage(
                    EnvironmentHelper.GetEnv("REQUEST_RESPONSE_LOGGING_CONNECTION_STRING"),
                    EnvironmentHelper.GetEnv("REQUEST_RESPONSE_LOGGING_CONTAINER_NAME"),
                    logger!);
                return storage;
            });
            serviceCollection.AddScoped<RequestResponseLoggingMiddleware>();

            // Health check
            serviceCollection.AddScoped<IHealthCheckEndpointHandler, HealthCheckEndpointHandler>();
            serviceCollection.AddHealthChecks()
                .AddLiveCheck()
                .AddAzureEventHub(name: "EventhubConnectionExists", eventHubConnectionFactory: options => new EventHubConnection(
                    EnvironmentHelper.GetEnv("EVENT_HUB_CONNECTION_STRING"),
                    EnvironmentHelper.GetEnv("EVENT_HUB_NAME")));
        }
    }
}
