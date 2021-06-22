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

using System.Diagnostics.CodeAnalysis;
using GreenEnergyHub.Messaging.Transport;
using GreenEnergyHub.TestHelpers;
using GreenEnergyHub.TimeSeries.Infrastructure.Messaging.Serialization;
using Moq;
using Xunit;
using Xunit.Categories;

namespace GreenEnergyHub.TimeSeries.Tests.Infrastructure.Messaging.Serialization
{
    [UnitTest]
    public class DefaultJsonMapperFactoryTests
    {
        [Theory]
        [InlineAutoDomainData]
        public void GetMapper_WhenCalled_ReturnsNoMapper(
            [NotNull] Mock<IOutboundMessage> input,
            [NotNull] DefaultJsonMapperFactory sut)
        {
            // Act
            var result = sut.GetMapper(input.Object);

            // Assert
            Assert.IsType<NoMapper>(result);
        }
    }
}
