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
from geh_stream.dataframelib import has_column
from geh_stream.validation import Validator


@pytest.fixture(scope="module")
def validated_data(spark, enriched_data):
    return Validator.add_validation_status_columns(enriched_data)


@pytest.fixture(scope="module")
def not_valid_validated_data(spark, non_enriched_data):
    return Validator.add_validation_status_columns(non_enriched_data)


def test_validator_drops_cols_only_needed_for_validation(enriched_data, validated_data):
    # Start by testing existence just in order to catch potential renamings and thus preventing false succeesses
    assert has_column(enriched_data, "md.meteringPointType")
    assert has_column(enriched_data, "md.settlementMethod")

    assert not has_column(validated_data, "md.meteringPointType")
    assert not has_column(validated_data, "md.settlementMethod")


def test_validator_adds_IsTimeSeriesPointValid_col(validated_data):
    assert has_column(validated_data, "IsTimeSeriesPointValid")


def test_validator_set_valid_IsTimeSeriesPointValid_col(validated_data):
    assert validated_data.first().IsTimeSeriesPointValid


def test_validator_set_invalid_IsTimeSeriesPointValid_col(not_valid_validated_data):
    assert not not_valid_validated_data.first().IsTimeSeriesPointValid


def test_validator_adds_vr200_col(validated_data):
    assert has_column(validated_data, "VR-200-Is-Valid")


def test_validator_adds_vr245_1_col(validated_data):
    assert has_column(validated_data, "VR-245-1-Is-Valid")


def test_validator_adds_vr250_col(validated_data):
    assert has_column(validated_data, "VR-250-Is-Valid")


def test_validator_adds_vr251_col(validated_data):
    assert has_column(validated_data, "VR-251-Is-Valid")


def test_validator_adds_vr611_col(validated_data):
    assert has_column(validated_data, "VR-611-Is-Valid")


def test_validator_adds_vr612_col(validated_data):
    assert has_column(validated_data, "VR-612-Is-Valid")
