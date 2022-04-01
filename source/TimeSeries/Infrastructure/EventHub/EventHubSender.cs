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
using Azure.Messaging.EventHubs.Producer;
using Energinet.DataHub.Core.App.FunctionApp.Middleware.CorrelationId;

namespace Energinet.DataHub.TimeSeries.Infrastructure.EventHub
{
    public class EventHubSender : IEventHubSender, IAsyncDisposable
    {
        private readonly IEventDataFactory _eventDataFactory;
        private readonly EventHubProducerClient _eventHubProducerClient;

        public EventHubSender(string connectionString, string eventHubName, IEventDataFactory eventDataFactory)
        {
            _eventDataFactory = eventDataFactory;
            _eventHubProducerClient = new EventHubProducerClient(connectionString, eventHubName);
        }

        public async ValueTask DisposeAsync()
        {
            await _eventHubProducerClient.DisposeAsync().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        public async Task SendAsync(byte[] body)
        {
            using var eventDataBatch =
                await _eventHubProducerClient.CreateBatchAsync().ConfigureAwait(false);
            var eventData = _eventDataFactory.Create(body);
            eventDataBatch.TryAdd(eventData);
            await _eventHubProducerClient.SendAsync(eventDataBatch).ConfigureAwait(false);
        }
    }
}
