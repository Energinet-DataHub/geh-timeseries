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
using Energinet.DataHub.Core.FunctionApp.Common.Middleware;
using Energinet.DataHub.Core.FunctionApp.Common.SimpleInjector;
using Energinet.DataHub.Core.Logging.RequestResponseMiddleware;
using Energinet.DataHub.Core.Logging.RequestResponseMiddleware.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleInjector;

namespace Energinet.DataHub.TimeSeries.MessageReceiver
{
    public class Program : Startup
    {
        public static async Task Main()
        {
            var program = new Program();

            var host = program.ConfigureApplication();
            program.VerifyContainer();
            await program.ExecuteApplicationAsync(host).ConfigureAwait(false);
        }

        protected override void ConfigureFunctionsWorkerDefaults(IFunctionsWorkerApplicationBuilder options)
        {
            base.ConfigureFunctionsWorkerDefaults(options);

            options.UseMiddleware<JwtTokenMiddleware>();
            options.UseMiddleware<RequestResponseLoggingMiddleware>();
        }

        protected override void ConfigureContainer(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            base.ConfigureContainer(container);

            container.AddJwtTokenSecurity(
                "https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration",
                "audience");

            container.RegisterSingleton<IRequestResponseLogging>(
                () =>
                {
                    var logger = container.GetService<ILogger<RequestResponseLoggingBlobStorage>>();
                    var storage = new RequestResponseLoggingBlobStorage(
                        Environment.GetEnvironmentVariable("REQUEST_RESPONSE_LOGGING_CONNECTION_STRING") ?? throw new InvalidOperationException(),
                        Environment.GetEnvironmentVariable("REQUEST_RESPONSE_LOGGING_CONTAINER_NAME") ?? throw new InvalidOperationException(),
                        logger ?? throw new InvalidOperationException());
                    return storage;
                });
            container.Register<RequestResponseLoggingMiddleware>(Lifestyle.Scoped);
        }
    }
}