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
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Energinet.DataHub.TimeSeries.MessageReceiver;

using System.Text.Json;

public class DatabricksClient
{
    private readonly string _token;

    public DatabricksClient(string token)
    {
        _token = token;
    }

    public async Task<Root?> GetClustersAsync()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://adb-5870161604877074.14.azuredatabricks.net");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

        using var response = await client.GetAsync("api/2.0/clusters/list");
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync();
        return JsonSerializer.Deserialize<Root>(stream);
    }
}

public class Root
{
    [JsonPropertyName("clusters")]
    public List<Cluster>? Clusters { get; set; }
}

public class Cluster
{
    [JsonPropertyName("cluster_id")]
    public string? ClusterId { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("default_tags")]
    public DefaultTags? DefaultTags { get; set; }
}

public class DefaultTags
{
    public string? ClusterName { get; set; }

    public string? JobId { get; set; }

    public string? RunName { get; set; }
}
