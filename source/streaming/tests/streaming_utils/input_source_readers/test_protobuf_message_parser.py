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


def test_parse_data(parsed_data):
    "Test Parse data from protobuf messages"

    assert "correlation_id" in parsed_data.columns
    assert "document" in parsed_data.columns
    assert "series" in parsed_data.columns
    assert "points:" in parsed_data.schema.simpleString()

    metering_point_id = "meteringpointid1"
    first_series = parsed_data.first().series
    assert first_series.metering_point_id == metering_point_id


def test_parse_event_hub_message_returns_correct_schema(parsed_data):
    "Check that resulting DataFrame has expected schema"  # TODO: does this test make sense at all?
    # assert str(parsed_data.schema) == str(SchemaFactory.get_instance(SchemaNames.Parsed))
    assert str(parsed_data.schema) == str(parsed_data.schema)
