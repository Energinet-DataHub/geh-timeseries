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

using System.Collections.Generic;

namespace GreenEnergyHub.Messaging
{
    /// <summary>
    /// Represents a response from Green Energy Hub
    /// </summary>
    public interface IHubResponse
    {
        /// <summary>
        /// Whether the action was successful.
        /// </summary>
        /// <value>True if the action was successful. Defaults to false.</value>
        bool IsSuccessful { get; set; }

        /// <summary>
        /// A list of any errors encountered.
        /// </summary>
        /// <value>The list of errors. Empty by default.</value>
        List<string> Errors { get; }
    }
}
