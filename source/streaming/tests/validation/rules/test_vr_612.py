# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
import pytest
from geh_stream.codelists import MeteringPointType, SettlementMethod
from geh_stream.validation.rules.vr_612 import validate_vr_612


@pytest.mark.parametrize(
    "quantity,metering_point_type,settlement_method,expected",
    [
        pytest.param(
            1E6 - 1,
            MeteringPointType.consumption.value,
            SettlementMethod.flex.value,
            True,
            id="valid because production limit is not exceeded"
        ),
        pytest.param(
            1E6,
            MeteringPointType.consumption.value,
            SettlementMethod.flex.value,
            False,
            id="invalid because production limit is exceeded"
        ),
        pytest.param(
            1E6,
            MeteringPointType.production.value,
            SettlementMethod.unknown.value,
            True,
            id="valid when exceeding limit because it's not a consumption metering point"
        ),
    ],
)
def test_vr_612(quantity, metering_point_type, settlement_method, expected, enriched_data_factory):
    data = enriched_data_factory(quantity=quantity,
                                 metering_point_type=metering_point_type,
                                 settlement_method=settlement_method)
    validated_data = validate_vr_612(data)
    assert validated_data.first()["VR-612-Is-Valid"] == expected
